using Application.Commond.Interface.ITenants;
using Application.Dtos;
using MediatR;

namespace Application.UserCase.V1.Tenants.Queries
{
    public class GetAllTenants : IRequest<IReadOnlyCollection<TenantResponseDto>>
    {
    }

    public class GetAllTenantsHandler(ITenantsCommandQuery tenantsCommandQuery) : IRequestHandler<GetAllTenants, IReadOnlyCollection<TenantResponseDto>>
    {
        public async Task<IReadOnlyCollection<TenantResponseDto>> Handle(GetAllTenants request, CancellationToken cancellationToken)
        {
            return await tenantsCommandQuery.GetAllTenantsAsync(cancellationToken);
        }
    }
}
