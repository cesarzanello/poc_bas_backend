using Application.Commond.Interface;
using Application.Commond.Interface.IProductos;
using MediatR;

namespace Application.UserCase.V1.Productos.Commands
{
    public class DeleteProducto : IRequest
    {
        public Guid ProductoId { get; set; }
    }

    public class DeleteProductoHandler(IProductosCommandQuery productosCommandQuery, INotificationsFacade notificationsFacade) : IRequestHandler<DeleteProducto>
    {
        public async Task Handle(DeleteProducto request, CancellationToken cancellationToken)
        {
            var producto = await productosCommandQuery.GetProductoByIdAsync(request.ProductoId, cancellationToken);
            if (producto is null)
                throw new KeyNotFoundException("No se encontró el producto indicado.");

            await productosCommandQuery.DeleteProductoAsync(request.ProductoId, cancellationToken);

            await notificationsFacade.BroadcastAsync(
                producto.TenantId,
                "Producto eliminado",
                $"Se eliminó el producto {request.ProductoId}.",
                "warning",
                cancellationToken,
                new { request.ProductoId, producto.TenantId });
        }
    }
}
