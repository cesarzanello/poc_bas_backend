using Application.Commond.Interface;
using Application.Commond.Interface.IProductos;
using Application.Dtos;
using MediatR;

namespace Application.UserCase.V1.Productos.Queries
{
    public class GetProductosByTenant : IRequest<IReadOnlyCollection<ProductoResponseDto>>
    {
        public Guid TenantId { get; set; }
    }

    public class GetProductosByTenantHandler(IProductosCommandQuery productosCommandQuery, INotificationsFacade notificationsFacade) : IRequestHandler<GetProductosByTenant, IReadOnlyCollection<ProductoResponseDto>>
    {
        public async Task<IReadOnlyCollection<ProductoResponseDto>> Handle(GetProductosByTenant request, CancellationToken cancellationToken)
        {
            var productos = await productosCommandQuery.GetProductosByTenantAsync(request.TenantId, cancellationToken);

            await notificationsFacade.BroadcastAsync(
                "Productos consultados",
                $"Se consultaron {productos.Count} productos del tenant {request.TenantId}.",
                "info",
                cancellationToken);

            return productos;
        }
    }
}
