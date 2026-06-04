using Application.Commond.Interface;
using Application.Commond.Interface.IProductos;
using Application.Dtos;
using MediatR;

namespace Application.UserCase.V1.Productos.Queries
{
    public class GetProductoById : IRequest<ProductoResponseDto?>
    {
        public Guid ProductoId { get; set; }
    }

    public class GetProductoByIdHandler(IProductosCommandQuery productosCommandQuery, INotificationsFacade notificationsFacade) : IRequestHandler<GetProductoById, ProductoResponseDto?>
    {
        public async Task<ProductoResponseDto?> Handle(GetProductoById request, CancellationToken cancellationToken)
        {
            var producto = await productosCommandQuery.GetProductoByIdAsync(request.ProductoId, cancellationToken);

            await notificationsFacade.BroadcastAsync(
                "Producto consultado",
                producto is null
                    ? $"Se consultó el producto {request.ProductoId} y no fue encontrado."
                    : $"Se consultó el producto {producto.Nombre} ({producto.Id}).",
                "info",
                cancellationToken);

            return producto;
        }
    }
}
