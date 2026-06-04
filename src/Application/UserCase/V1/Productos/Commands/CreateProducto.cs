using Application.Commond.Interface;
using Application.Commond.Interface.IProductos;
using Application.Dtos;
using MediatR;

namespace Application.UserCase.V1.Productos.Commands
{
    public class CreateProducto : IRequest<ProductoResponseDto>
    {
        public CreateProductoRequestDto Request { get; set; } = new();
    }

    public class CreateProductoHandler(IProductosCommandQuery productosCommandQuery, INotificationsFacade notificationsFacade) : IRequestHandler<CreateProducto, ProductoResponseDto>
    {
        public async Task<ProductoResponseDto> Handle(CreateProducto request, CancellationToken cancellationToken)
        {
            var productoId = Guid.NewGuid();
            var producto = await productosCommandQuery.CreateProductoAsync(productoId, request.Request, cancellationToken);

            await notificationsFacade.BroadcastAsync(
                "Producto creado",
                $"Se creó el producto {producto.Nombre} ({producto.Id}).",
                "success",
                cancellationToken);

            return producto;
        }
    }
}
