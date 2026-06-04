using Application.Commond.Interface;
using Application.Commond.Interface.ITenants;
using Application.Dtos;
using MediatR;

namespace Application.UserCase.V1.Tenants.Commands
{
    public class CreateTenant : IRequest<TenantResponseDto>
    {
        public string Nombre { get; set; } = string.Empty;
    }

    public class CreateTenantHandler(ITenantsCommandQuery tenantsCommandQuery, INotificationsFacade notificationsFacade) : IRequestHandler<CreateTenant, TenantResponseDto>
    {
        public async Task<TenantResponseDto> Handle(CreateTenant request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                throw new ArgumentException("El nombre del tenant es obligatorio.");

            var tenantId = Guid.NewGuid();
            var tenant = await tenantsCommandQuery.CreateTenantAsync(tenantId, request.Nombre.Trim(), cancellationToken);

            await notificationsFacade.BroadcastAsync(
                "Tenant creado",
                $"Se creó el tenant {tenant.Nombre} ({tenant.Id}).",
                "success",
                cancellationToken);

            return tenant;
        }
    }
}
