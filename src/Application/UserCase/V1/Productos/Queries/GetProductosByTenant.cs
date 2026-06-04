using Application.Commond.Interface.IProductos;
using Application.Dtos;
using MediatR;

namespace Application.UserCase.V1.Productos.Queries
{
    public class GetProductosByTenant : IRequest<IReadOnlyCollection<ProductoResponseDto>>
    {
        public Guid TenantId { get; set; }
    }

    public class GetProductosByTenantHandler(IProductosCommandQuery productosCommandQuery) : IRequestHandler<GetProductosByTenant, IReadOnlyCollection<ProductoResponseDto>>
    {
        public async Task<IReadOnlyCollection<ProductoResponseDto>> Handle(GetProductosByTenant request, CancellationToken cancellationToken)
        {
            return await productosCommandQuery.GetProductosByTenantAsync(request.TenantId, cancellationToken);
        }
    }
}
