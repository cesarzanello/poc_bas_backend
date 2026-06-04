using Application.Commond.Interface;
using Application.Commond.Interface.IProductos;
using Application.Dtos;
using MediatR;

namespace Application.UserCase.V1.Productos.Commands
{
    public class UpdateProducto : IRequest<ProductoResponseDto>
    {
        public Guid ProductoId { get; set; }

        public UpdateProductoRequestDto Request { get; set; } = new();
    }

    public class UpdateProductoHandler(IProductosCommandQuery productosCommandQuery, INotificationsFacade notificationsFacade) : IRequestHandler<UpdateProducto, ProductoResponseDto>
    {
        public async Task<ProductoResponseDto> Handle(UpdateProducto request, CancellationToken cancellationToken)
        {
            var producto = await productosCommandQuery.UpdateProductoAsync(request.ProductoId, request.Request, cancellationToken);

            await notificationsFacade.BroadcastAsync(
                "Producto actualizado",
                $"Se actualizó el producto {producto.Nombre} ({producto.Id}).",
                "info",
                cancellationToken);

            return producto;
        }
    }
}
