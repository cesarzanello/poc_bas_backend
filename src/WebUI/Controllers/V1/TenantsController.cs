using Application.Dtos;
using Application.UserCase.V1.Tenants.Commands;
using Application.UserCase.V1.Tenants.Queries;
using Microsoft.AspNetCore.Mvc;
using WebUI.Base;

namespace WebUI.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TenantsController : ApiControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(TenantResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantRequestDto request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
            {
                return BadRequest("El nombre del tenant es obligatorio.");
            }

            var tenant = await Mediator.Send(new CreateTenant
            {
                Nombre = request.Nombre.Trim()
            }, cancellationToken);

            return CreatedAtAction(nameof(GetAllTenants), tenant);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<TenantResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTenants(CancellationToken cancellationToken)
        {
            var tenants = await Mediator.Send(new GetAllTenants(), cancellationToken);
            return Ok(tenants);
        }
    }
}
