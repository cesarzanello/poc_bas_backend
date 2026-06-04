using Application.Commond.Interface;
using Application.Commond.Interface.IProductos;
using Application.Commond.Interface.ITenants;
using Application.Dtos;
using Application.UserCase.V1.Productos.Commands;
using Application.UserCase.V1.Productos.Queries;
using Moq;
using Xunit;

namespace Application.Test.UserCase.V1.Productos;

public class ProductoHandlersTests
{
    [Fact]
    public async Task CreateProductoHandler_Should_Create_Producto_And_Broadcast()
    {
        var productos = new Mock<IProductosCommandQuery>();
        var tenants = new Mock<ITenantsCommandQuery>();
        var notifications = new Mock<INotificationsFacade>();
        var cancellationToken = new CancellationTokenSource().Token;

        var request = new CreateProductoRequestDto
        {
            TenantId = Guid.NewGuid(),
            Codigo = "P-001",
            Nombre = "Producto 1",
            Precio = 10
        };

        var expected = new ProductoResponseDto
        {
            Id = Guid.NewGuid(),
            TenantId = request.TenantId,
            Codigo = request.Codigo,
            Nombre = request.Nombre,
            Precio = request.Precio
        };

        tenants
            .Setup(x => x.ExistsTenantAsync(request.TenantId, cancellationToken))
            .ReturnsAsync(true);

        Guid createdId = Guid.Empty;
        productos
            .Setup(x => x.CreateProductoAsync(It.IsAny<Guid>(), request, cancellationToken))
            .Callback<Guid, CreateProductoRequestDto, CancellationToken>((id, _, _) => createdId = id)
            .ReturnsAsync(expected);

        var handler = new CreateProductoHandler(productos.Object, tenants.Object, notifications.Object);

        var result = await handler.Handle(new CreateProducto { Request = request }, cancellationToken);

        Assert.Equal(expected, result);
        Assert.NotEqual(Guid.Empty, createdId);

        notifications.Verify(x => x.BroadcastAsync(
            "Producto creado",
            It.Is<string>(m => m.Contains(expected.Nombre) && m.Contains(expected.Id.ToString())),
            "success",
            cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CreateProductoHandler_Should_Throw_ArgumentException_When_TenantId_Empty()
    {
        var productos = new Mock<IProductosCommandQuery>();
        var tenants = new Mock<ITenantsCommandQuery>();
        var notifications = new Mock<INotificationsFacade>();

        var handler = new CreateProductoHandler(productos.Object, tenants.Object, notifications.Object);
        var request = new CreateProductoRequestDto { TenantId = Guid.Empty, Codigo = "P-001", Nombre = "Test", Precio = 1 };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.Handle(new CreateProducto { Request = request }, CancellationToken.None));
    }

    [Theory]
    [InlineData("", "Nombre")]
    [InlineData("   ", "Nombre")]
    [InlineData("Codigo", "")]
    [InlineData("Codigo", "   ")]
    public async Task CreateProductoHandler_Should_Throw_ArgumentException_When_Codigo_Or_Nombre_Empty(string codigo, string nombre)
    {
        var productos = new Mock<IProductosCommandQuery>();
        var tenants = new Mock<ITenantsCommandQuery>();
        var notifications = new Mock<INotificationsFacade>();

        var handler = new CreateProductoHandler(productos.Object, tenants.Object, notifications.Object);
        var request = new CreateProductoRequestDto { TenantId = Guid.NewGuid(), Codigo = codigo, Nombre = nombre, Precio = 1 };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.Handle(new CreateProducto { Request = request }, CancellationToken.None));
    }

    [Fact]
    public async Task CreateProductoHandler_Should_Throw_KeyNotFoundException_When_Tenant_Not_Found()
    {
        var productos = new Mock<IProductosCommandQuery>();
        var tenants = new Mock<ITenantsCommandQuery>();
        var notifications = new Mock<INotificationsFacade>();
        var cancellationToken = new CancellationTokenSource().Token;

        var tenantId = Guid.NewGuid();
        tenants.Setup(x => x.ExistsTenantAsync(tenantId, cancellationToken)).ReturnsAsync(false);

        var handler = new CreateProductoHandler(productos.Object, tenants.Object, notifications.Object);
        var request = new CreateProductoRequestDto { TenantId = tenantId, Codigo = "P-001", Nombre = "Test", Precio = 1 };

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new CreateProducto { Request = request }, cancellationToken));
    }

    [Fact]
    public async Task UpdateProductoHandler_Should_Update_Producto_And_Broadcast()
    {
        var productos = new Mock<IProductosCommandQuery>();
        var notifications = new Mock<INotificationsFacade>();
        var cancellationToken = new CancellationTokenSource().Token;

        var productoId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var updateRequest = new UpdateProductoRequestDto
        {
            Codigo = "P-002",
            Nombre = "Producto actualizado",
            Precio = 20
        };

        var expected = new ProductoResponseDto
        {
            Id = productoId,
            TenantId = tenantId,
            Codigo = updateRequest.Codigo,
            Nombre = updateRequest.Nombre,
            Precio = updateRequest.Precio
        };

        productos
            .Setup(x => x.GetProductoByIdAsync(productoId, cancellationToken))
            .ReturnsAsync(expected);

        productos
            .Setup(x => x.ExistsCodigoForTenantAsync(tenantId, updateRequest.Codigo, productoId, cancellationToken))
            .ReturnsAsync(false);

        productos
            .Setup(x => x.UpdateProductoAsync(productoId, updateRequest, cancellationToken))
            .ReturnsAsync(expected);

        var handler = new UpdateProductoHandler(productos.Object, notifications.Object);

        var result = await handler.Handle(new UpdateProducto { ProductoId = productoId, Request = updateRequest }, cancellationToken);

        Assert.Equal(expected, result);

        notifications.Verify(x => x.BroadcastAsync(
            "Producto actualizado",
            It.Is<string>(m => m.Contains(expected.Nombre) && m.Contains(expected.Id.ToString())),
            "info",
            cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpdateProductoHandler_Should_Throw_ArgumentException_When_ProductoId_Empty()
    {
        var productos = new Mock<IProductosCommandQuery>();
        var notifications = new Mock<INotificationsFacade>();

        var handler = new UpdateProductoHandler(productos.Object, notifications.Object);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.Handle(new UpdateProducto { ProductoId = Guid.Empty, Request = new UpdateProductoRequestDto { Codigo = "C", Nombre = "N", Precio = 1 } }, CancellationToken.None));
    }

    [Theory]
    [InlineData("", "Nombre")]
    [InlineData("Codigo", "")]
    public async Task UpdateProductoHandler_Should_Throw_ArgumentException_When_Codigo_Or_Nombre_Empty(string codigo, string nombre)
    {
        var productos = new Mock<IProductosCommandQuery>();
        var notifications = new Mock<INotificationsFacade>();

        var handler = new UpdateProductoHandler(productos.Object, notifications.Object);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.Handle(new UpdateProducto { ProductoId = Guid.NewGuid(), Request = new UpdateProductoRequestDto { Codigo = codigo, Nombre = nombre, Precio = 1 } }, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateProductoHandler_Should_Throw_KeyNotFoundException_When_Producto_Not_Found()
    {
        var productos = new Mock<IProductosCommandQuery>();
        var notifications = new Mock<INotificationsFacade>();
        var cancellationToken = new CancellationTokenSource().Token;

        var productoId = Guid.NewGuid();
        productos.Setup(x => x.GetProductoByIdAsync(productoId, cancellationToken)).ReturnsAsync((ProductoResponseDto?)null);

        var handler = new UpdateProductoHandler(productos.Object, notifications.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new UpdateProducto { ProductoId = productoId, Request = new UpdateProductoRequestDto { Codigo = "C", Nombre = "N", Precio = 1 } }, cancellationToken));
    }

    [Fact]
    public async Task UpdateProductoHandler_Should_Throw_InvalidOperationException_When_Codigo_Duplicated()
    {
        var productos = new Mock<IProductosCommandQuery>();
        var notifications = new Mock<INotificationsFacade>();
        var cancellationToken = new CancellationTokenSource().Token;

        var productoId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var request = new UpdateProductoRequestDto { Codigo = "DUP", Nombre = "Nombre", Precio = 1 };

        productos
            .Setup(x => x.GetProductoByIdAsync(productoId, cancellationToken))
            .ReturnsAsync(new ProductoResponseDto { Id = productoId, TenantId = tenantId, Codigo = "OLD", Nombre = "Nombre", Precio = 1 });

        productos
            .Setup(x => x.ExistsCodigoForTenantAsync(tenantId, request.Codigo, productoId, cancellationToken))
            .ReturnsAsync(true);

        var handler = new UpdateProductoHandler(productos.Object, notifications.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new UpdateProducto { ProductoId = productoId, Request = request }, cancellationToken));
    }

    [Fact]
    public async Task DeleteProductoHandler_Should_Delete_Producto_And_Broadcast()
    {
        var productos = new Mock<IProductosCommandQuery>();
        var notifications = new Mock<INotificationsFacade>();
        var cancellationToken = new CancellationTokenSource().Token;

        var productoId = Guid.NewGuid();

        productos
            .Setup(x => x.ExistsProductoAsync(productoId, cancellationToken))
            .ReturnsAsync(true);

        productos
            .Setup(x => x.DeleteProductoAsync(productoId, cancellationToken))
            .Returns(Task.CompletedTask);

        var handler = new DeleteProductoHandler(productos.Object, notifications.Object);

        await handler.Handle(new DeleteProducto { ProductoId = productoId }, cancellationToken);

        productos.Verify(x => x.DeleteProductoAsync(productoId, cancellationToken), Times.Once);
        notifications.Verify(x => x.BroadcastAsync(
            "Producto eliminado",
            It.Is<string>(m => m.Contains(productoId.ToString())),
            "warning",
            cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteProductoHandler_Should_Throw_KeyNotFoundException_When_Producto_Not_Found()
    {
        var productos = new Mock<IProductosCommandQuery>();
        var notifications = new Mock<INotificationsFacade>();
        var cancellationToken = new CancellationTokenSource().Token;

        var productoId = Guid.NewGuid();
        productos.Setup(x => x.ExistsProductoAsync(productoId, cancellationToken)).ReturnsAsync(false);

        var handler = new DeleteProductoHandler(productos.Object, notifications.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new DeleteProducto { ProductoId = productoId }, cancellationToken));
    }

    [Fact]
    public async Task GetProductoByIdHandler_Should_Return_Producto_And_Broadcast_Found_Message()
    {
        var productos = new Mock<IProductosCommandQuery>();
        var notifications = new Mock<INotificationsFacade>();
        var cancellationToken = new CancellationTokenSource().Token;

        var productoId = Guid.NewGuid();
        var expected = new ProductoResponseDto
        {
            Id = productoId,
            TenantId = Guid.NewGuid(),
            Codigo = "P-003",
            Nombre = "Producto consultado",
            Precio = 50
        };

        productos
            .Setup(x => x.GetProductoByIdAsync(productoId, cancellationToken))
            .ReturnsAsync(expected);

        var handler = new GetProductoByIdHandler(productos.Object, notifications.Object);

        var result = await handler.Handle(new GetProductoById { ProductoId = productoId }, cancellationToken);

        Assert.Equal(expected, result);
        notifications.Verify(x => x.BroadcastAsync(
            "Producto consultado",
            It.Is<string>(m => m.Contains(expected.Nombre) && m.Contains(expected.Id.ToString())),
            "info",
            cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetProductoByIdHandler_Should_Return_Null_And_Broadcast_NotFound_Message()
    {
        var productos = new Mock<IProductosCommandQuery>();
        var notifications = new Mock<INotificationsFacade>();
        var cancellationToken = new CancellationTokenSource().Token;

        var productoId = Guid.NewGuid();

        productos
            .Setup(x => x.GetProductoByIdAsync(productoId, cancellationToken))
            .ReturnsAsync((ProductoResponseDto?)null);

        var handler = new GetProductoByIdHandler(productos.Object, notifications.Object);

        var result = await handler.Handle(new GetProductoById { ProductoId = productoId }, cancellationToken);

        Assert.Null(result);
        notifications.Verify(x => x.BroadcastAsync(
            "Producto consultado",
            It.Is<string>(m => m.Contains(productoId.ToString()) && m.Contains("no fue encontrado")),
            "info",
            cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetProductosByTenantHandler_Should_Return_Items_And_Broadcast_Count()
    {
        var productos = new Mock<IProductosCommandQuery>();
        var notifications = new Mock<INotificationsFacade>();
        var cancellationToken = new CancellationTokenSource().Token;

        var tenantId = Guid.NewGuid();
        IReadOnlyCollection<ProductoResponseDto> expected =
        [
            new ProductoResponseDto { Id = Guid.NewGuid(), TenantId = tenantId, Codigo = "P-01", Nombre = "A", Precio = 1 },
            new ProductoResponseDto { Id = Guid.NewGuid(), TenantId = tenantId, Codigo = "P-02", Nombre = "B", Precio = 2 }
        ];

        productos
            .Setup(x => x.GetProductosByTenantAsync(tenantId, cancellationToken))
            .ReturnsAsync(expected);

        var handler = new GetProductosByTenantHandler(productos.Object, notifications.Object);

        var result = await handler.Handle(new GetProductosByTenant { TenantId = tenantId }, cancellationToken);

        Assert.Equal(expected, result);
        notifications.Verify(x => x.BroadcastAsync(
            "Productos consultados",
            It.Is<string>(m => m.Contains("2") && m.Contains(tenantId.ToString())),
            "info",
            cancellationToken), Times.Once);
    }
}
