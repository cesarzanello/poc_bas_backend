using Application.Dtos;

namespace Application.Commond.Interface.ITenants
{
    public interface ITenantsCommandQuery : IScopedService
    {
        Task<TenantResponseDto> CreateTenantAsync(Guid tenantId, string nombre, CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<TenantResponseDto>> GetAllTenantsAsync(CancellationToken cancellationToken = default);

        Task<bool> ExistsTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    }
}
