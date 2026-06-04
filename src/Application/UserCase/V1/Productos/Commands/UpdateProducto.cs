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
            if (request.ProductoId == Guid.Empty)
                throw new ArgumentException("El productoId es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Request.Codigo) || string.IsNullOrWhiteSpace(request.Request.Nombre))
                throw new ArgumentException("Código y nombre son obligatorios.");

            var productoActual = await productosCommandQuery.GetProductoByIdAsync(request.ProductoId, cancellationToken);
            if (productoActual is null)
                throw new KeyNotFoundException("No se encontró el producto indicado.");

            var codigoExists = await productosCommandQuery.ExistsCodigoForTenantAsync(productoActual.TenantId, request.Request.Codigo, request.ProductoId, cancellationToken);
            if (codigoExists)
                throw new InvalidOperationException("Ya existe un producto con el mismo código para el tenant indicado.");

            var producto = await productosCommandQuery.UpdateProductoAsync(request.ProductoId, request.Request, cancellationToken);

            await notificationsFacade.BroadcastAsync(
                "Producto actualizado",
                $"Se actualizó el producto {producto.Nombre} ({producto.Id}).",
                "info",
                cancellationToken,
                producto);

            return producto;
        }
    }
}
