using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Commond.Interface;

namespace WebUI.Base
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        [FromServices]
        public IMediator Mediator { get; set; } = default!;

        [FromServices]
        public INotificationService NotificationService { get; set; } = default!;
    }
}
