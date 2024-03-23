using Inventory_Management_System.Application.Common.Response;
using Inventory_Management_System.Application.Features.UserFeatures.Commands;
using Inventory_Management_System.Application.Features.UserFeatures.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management_System.API.Controllers
{
    public class UserController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserController> _logger;

        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("RegisteredUsers")]
        public async Task<ActionResult<ResponseVm<RegisteredUserVm>>> RegisteredUsers([FromBody] RegisteredUserCommand command)
        {
            return HandleResult(await _mediator.Send(command));
        }

        [HttpGet("Login")]
        public async Task<ActionResult<ResponseVm<LoginVM>>> Login([FromQuery] GetLoginUserQuery req)
        {
            var loginResult = await Mediator.Send(req);
            return HandleResult(loginResult);
        }
    }
}
