using Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using Application.UserCase.V1.Productos.Commands;
using Application.UserCase.V1.Productos.Queries;
using WebUI.Base;

namespace WebUI.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductosController : ApiControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateProducto([FromBody] CreateProductoRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                var producto = await Mediator.Send(new CreateProducto
                {
                    Request = request
                }, cancellationToken);
                return CreatedAtAction(nameof(GetProductoById), new { productoId = producto.Id }, producto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{productoId:guid}")]
        [ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProducto(Guid productoId, [FromBody] UpdateProductoRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                var producto = await Mediator.Send(new UpdateProducto
                {
                    ProductoId = productoId,
                    Request = request
                }, cancellationToken);
                return Ok(producto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{productoId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProducto(Guid productoId, CancellationToken cancellationToken)
        {
            try
            {
                await Mediator.Send(new DeleteProducto
                {
                    ProductoId = productoId
                }, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{productoId:guid}")]
        [ProducesResponseType(typeof(ProductoResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductoById(Guid productoId, CancellationToken cancellationToken)
        {
            var producto = await Mediator.Send(new GetProductoById
            {
                ProductoId = productoId
            }, cancellationToken);
            if (producto is null)
            {
                return NotFound("No se encontró el producto indicado.");
            }

            return Ok(producto);
        }

        [HttpGet("tenant/{tenantId:guid}")]
        [ProducesResponseType(typeof(IReadOnlyCollection<ProductoResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProductosByTenant(Guid tenantId, CancellationToken cancellationToken)
        {
            var productos = await Mediator.Send(new GetProductosByTenant
            {
                TenantId = tenantId
            }, cancellationToken);
            return Ok(productos);
        }
    }
}
