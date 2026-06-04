using Application.Commond.Interface;
using Application.Commond.Interface.ITenants;
using Application.Dtos;
using MediatR;

namespace Application.UserCase.V1.Tenants.Queries
{
    public class GetAllTenants : IRequest<IReadOnlyCollection<TenantResponseDto>>
    {
    }

    public class GetAllTenantsHandler(ITenantsCommandQuery tenantsCommandQuery, INotificationsFacade notificationsFacade) : IRequestHandler<GetAllTenants, IReadOnlyCollection<TenantResponseDto>>
    {
        public async Task<IReadOnlyCollection<TenantResponseDto>> Handle(GetAllTenants request, CancellationToken cancellationToken)
        {
            var tenants = await tenantsCommandQuery.GetAllTenantsAsync(cancellationToken);

            await notificationsFacade.BroadcastAsync(
                "Tenants consultados",
                $"Se consultaron {tenants.Count} tenants.",
                "info",
                cancellationToken);

            return tenants;
        }
    }
}
