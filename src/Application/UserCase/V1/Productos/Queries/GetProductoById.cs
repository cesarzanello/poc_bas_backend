using Application.Commond.Interface.IProductos;
using Application.Dtos;
using MediatR;

namespace Application.UserCase.V1.Productos.Queries
{
    public class GetProductoById : IRequest<ProductoResponseDto?>
    {
        public Guid ProductoId { get; set; }
    }

    public class GetProductoByIdHandler(IProductosCommandQuery productosCommandQuery) : IRequestHandler<GetProductoById, ProductoResponseDto?>
    {
        public async Task<ProductoResponseDto?> Handle(GetProductoById request, CancellationToken cancellationToken)
        {
            return await productosCommandQuery.GetProductoByIdAsync(request.ProductoId, cancellationToken);
        }
    }
}
