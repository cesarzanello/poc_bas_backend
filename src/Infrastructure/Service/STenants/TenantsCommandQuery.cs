using Application.Commond.Interface.ITenants;
using Application.Dtos;
using Infrastructure.Persistence.Mongo;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Infrastructure.Service.STenants
{
    public class TenantsCommandQuery(IMongoCollection<TenantDocument> tenantsCollection, ILogger<TenantsCommandQuery> logger) : ITenantsCommandQuery
    {
        public async Task<TenantResponseDto> CreateTenantAsync(Guid tenantId, string nombre, CancellationToken cancellationToken = default)
        {
            try
            {
                var tenant = new TenantDocument
                {
                    Id = tenantId,
                    Nombre = nombre
                };

                await tenantsCollection.InsertOneAsync(tenant, cancellationToken: cancellationToken);

                return new TenantResponseDto
                {
                    Id = tenant.Id,
                    Nombre = tenant.Nombre
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creando tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<IReadOnlyCollection<TenantResponseDto>> GetAllTenantsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var tenants = await tenantsCollection
                    .Find(_ => true)
                    .SortBy(x => x.Nombre)
                    .ToListAsync(cancellationToken);

                return tenants
                    .Select(x => new TenantResponseDto
                    {
                        Id = x.Id,
                        Nombre = x.Nombre
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error obteniendo todos los tenants");
                throw;
            }
        }

        public async Task<bool> ExistsTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                var count = await tenantsCollection.CountDocumentsAsync(x => x.Id == tenantId, cancellationToken: cancellationToken);
                return count > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error validando tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}
