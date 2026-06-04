using Application.Commond.Interface;
using Application.Commond.Interface.IProductos;
using Application.Commond.Interface.ITenants;
using Application.Dtos;
using MediatR;

namespace Application.UserCase.V1.Productos.Commands
{
    public class CreateProducto : IRequest<ProductoResponseDto>
    {
        public CreateProductoRequestDto Request { get; set; } = new();
    }

    public class CreateProductoHandler(IProductosCommandQuery productosCommandQuery, ITenantsCommandQuery tenantsCommandQuery, INotificationsFacade notificationsFacade) : IRequestHandler<CreateProducto, ProductoResponseDto>
    {
        public async Task<ProductoResponseDto> Handle(CreateProducto request, CancellationToken cancellationToken)
        {
            if (request.Request.TenantId == Guid.Empty)
                throw new ArgumentException("El tenantId es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Request.Codigo) || string.IsNullOrWhiteSpace(request.Request.Nombre))
                throw new ArgumentException("Código y nombre son obligatorios.");

            var tenantExists = await tenantsCommandQuery.ExistsTenantAsync(request.Request.TenantId, cancellationToken);
            if (!tenantExists)
                throw new KeyNotFoundException("No se encontró el tenant indicado.");

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
