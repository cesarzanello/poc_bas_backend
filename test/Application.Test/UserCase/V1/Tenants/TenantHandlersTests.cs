using Application.Commond.Interface;
using Application.Commond.Interface.ITenants;
using Application.Dtos;
using Application.UserCase.V1.Tenants.Commands;
using Application.UserCase.V1.Tenants.Queries;
using Moq;
using Xunit;

namespace Application.Test.UserCase.V1.Tenants;

public class TenantHandlersTests
{
    [Fact]
    public async Task CreateTenantHandler_Should_Create_Tenant_And_Broadcast()
    {
        var tenants = new Mock<ITenantsCommandQuery>();
        var notifications = new Mock<INotificationsFacade>();
        var cancellationToken = new CancellationTokenSource().Token;

        var tenantName = "Tenant A";
        var expected = new TenantResponseDto
        {
            Id = Guid.NewGuid(),
            Nombre = tenantName
        };

        Guid createdId = Guid.Empty;
        tenants
            .Setup(x => x.CreateTenantAsync(It.IsAny<Guid>(), tenantName, cancellationToken))
            .Callback<Guid, string, CancellationToken>((id, _, _) => createdId = id)
            .ReturnsAsync(expected);

        var handler = new CreateTenantHandler(tenants.Object, notifications.Object);

        var result = await handler.Handle(new CreateTenant { Nombre = tenantName }, cancellationToken);

        Assert.Equal(expected, result);
        Assert.NotEqual(Guid.Empty, createdId);

        notifications.Verify(x => x.BroadcastAsync(
            expected.Id,
            "Tenant creado",
            It.Is<string>(m => m.Contains(expected.Nombre) && m.Contains(expected.Id.ToString())),
            "success",
            cancellationToken,
            It.Is<object?>(d => ReferenceEquals(d, expected))), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateTenantHandler_Should_Throw_ArgumentException_When_Nombre_Empty(string nombre)
    {
        var tenants = new Mock<ITenantsCommandQuery>();
        var notifications = new Mock<INotificationsFacade>();

        var handler = new CreateTenantHandler(tenants.Object, notifications.Object);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.Handle(new CreateTenant { Nombre = nombre }, CancellationToken.None));
    }

    [Fact]
    public async Task CreateTenantHandler_Should_Trim_Nombre_Before_Persisting()
    {
        var tenants = new Mock<ITenantsCommandQuery>();
        var notifications = new Mock<INotificationsFacade>();
        var cancellationToken = new CancellationTokenSource().Token;

        var expected = new TenantResponseDto { Id = Guid.NewGuid(), Nombre = "Tenant B" };

        tenants
            .Setup(x => x.CreateTenantAsync(It.IsAny<Guid>(), "Tenant B", cancellationToken))
            .ReturnsAsync(expected);

        var handler = new CreateTenantHandler(tenants.Object, notifications.Object);

        var result = await handler.Handle(new CreateTenant { Nombre = "  Tenant B  " }, cancellationToken);

        Assert.Equal(expected, result);
        tenants.Verify(x => x.CreateTenantAsync(It.IsAny<Guid>(), "Tenant B", cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetAllTenantsHandler_Should_Return_Items()
    {
        var tenants = new Mock<ITenantsCommandQuery>();
        var cancellationToken = new CancellationTokenSource().Token;

        IReadOnlyCollection<TenantResponseDto> expected =
        [
            new TenantResponseDto { Id = Guid.NewGuid(), Nombre = "Tenant A" },
            new TenantResponseDto { Id = Guid.NewGuid(), Nombre = "Tenant B" }
        ];

        tenants
            .Setup(x => x.GetAllTenantsAsync(cancellationToken))
            .ReturnsAsync(expected);

        var handler = new GetAllTenantsHandler(tenants.Object);

        var result = await handler.Handle(new GetAllTenants(), cancellationToken);

        Assert.Equal(expected, result);
    }
}
