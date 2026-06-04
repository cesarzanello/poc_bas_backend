using Application.Commond.Interface;
using Application.Commond.Interface.ITenants;
using Application.Dtos;
using Infrastructure.Persistence;
using Infrastructure.Service.SProductos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Domain.Entities;
using Xunit;

namespace Application.Test.Infrastructure.Service.SProductos;

public class ProductosCommandQueryUnitOfWorkTests
{
    [Fact]
    public async Task CreateProductoAsync_Should_Call_UnitOfWork_SaveChanges()
    {
        await using var dbContext = CreateDbContext();

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var tenants = new Mock<ITenantsCommandQuery>();
        tenants
            .Setup(x => x.ExistsTenantAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var logger = new Mock<ILogger<ProductosCommandQuery>>();

        var sut = new ProductosCommandQuery(dbContext, unitOfWork.Object, tenants.Object, logger.Object);
        var request = new CreateProductoRequestDto
        {
            TenantId = Guid.NewGuid(),
            Codigo = "P-100",
            Nombre = "Producto Test",
            Precio = 10
        };

        await sut.CreateProductoAsync(Guid.NewGuid(), request, CancellationToken.None);

        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProductoAsync_Should_Call_UnitOfWork_SaveChanges()
    {
        await using var dbContext = CreateDbContext();

        var productoId = Guid.NewGuid();
        dbContext.Productos.Add(new Producto
        {
            Id = productoId,
            TenantId = Guid.NewGuid(),
            Codigo = "P-200",
            Nombre = "Original",
            Precio = 10
        });
        await dbContext.SaveChangesAsync();

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var tenants = new Mock<ITenantsCommandQuery>();
        var logger = new Mock<ILogger<ProductosCommandQuery>>();

        var sut = new ProductosCommandQuery(dbContext, unitOfWork.Object, tenants.Object, logger.Object);
        var request = new UpdateProductoRequestDto
        {
            Codigo = "P-201",
            Nombre = "Actualizado",
            Precio = 20
        };

        await sut.UpdateProductoAsync(productoId, request, CancellationToken.None);

        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProductoAsync_Should_Call_UnitOfWork_SaveChanges()
    {
        await using var dbContext = CreateDbContext();

        var productoId = Guid.NewGuid();
        dbContext.Productos.Add(new Producto
        {
            Id = productoId,
            TenantId = Guid.NewGuid(),
            Codigo = "P-300",
            Nombre = "A eliminar",
            Precio = 5
        });
        await dbContext.SaveChangesAsync();

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var tenants = new Mock<ITenantsCommandQuery>();
        var logger = new Mock<ILogger<ProductosCommandQuery>>();

        var sut = new ProductosCommandQuery(dbContext, unitOfWork.Object, tenants.Object, logger.Object);

        await sut.DeleteProductoAsync(productoId, CancellationToken.None);

        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
