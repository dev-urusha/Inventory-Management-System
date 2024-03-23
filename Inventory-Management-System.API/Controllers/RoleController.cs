using Inventory_Management_System.Application.Common.Response;
using Inventory_Management_System.Application.Features.RoleFeatures.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management_System.API.Controllers
{
    public class RoleController : ApiControllerBase
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IMediator _mediator;

        public RoleController(ILogger<RoleController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("SaveRole")]
        public async Task<ActionResult<ResponseVm<SaveRoleCommandVm>>> SaveRole(SaveRoleCommand command)
        {
            return HandleResult(await _mediator.Send(command));
        }
    }
}
