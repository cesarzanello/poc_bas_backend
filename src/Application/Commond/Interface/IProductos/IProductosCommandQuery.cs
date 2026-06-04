using Application.Dtos;

namespace Application.Commond.Interface.IProductos
{
    public interface IProductosCommandQuery : IScopedService
    {
        Task<ProductoResponseDto> CreateProductoAsync(Guid productoId, CreateProductoRequestDto request, CancellationToken cancellationToken = default);

        Task<ProductoResponseDto> UpdateProductoAsync(Guid productoId, UpdateProductoRequestDto request, CancellationToken cancellationToken = default);

        Task DeleteProductoAsync(Guid productoId, CancellationToken cancellationToken = default);

        Task<ProductoResponseDto?> GetProductoByIdAsync(Guid productoId, CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<ProductoResponseDto>> GetProductosByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);

        Task<bool> ExistsProductoAsync(Guid productoId, CancellationToken cancellationToken = default);

        Task<bool> ExistsCodigoForTenantAsync(Guid tenantId, string codigo, Guid excludeProductoId, CancellationToken cancellationToken = default);
    }
}
