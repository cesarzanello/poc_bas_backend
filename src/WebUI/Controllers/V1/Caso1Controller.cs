using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Application.Dtos;
using MediatR;
using Application.UserCase.V1.Caso1.Queries;
using WebUI.Base;


namespace WebUI.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class Caso1Controller : ApiControllerBase   
    {  
        /// <summary>  
        /// Obtencion de los casos por medio del id
        /// </summary>
        /// <param name="caso1id"></param>
        /// <returns> id, nombre</returns>
        [HttpGet("Get/{caso1id}")]
        [ProducesResponseType(typeof(Caso1ResponseDto), StatusCodes.Status200OK)] 
        public async Task<IActionResult> GetCaso1xId(Guid caso1id)
        {
            var result = await Mediator.Send(new GetCaso1 { Id = caso1id });
            await NotificationService.NotifyAllAsync("Caso1Consultado", result);
            return Ok(result);
        }
    }
}
