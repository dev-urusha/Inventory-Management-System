using Inventory_Management_System.Application.Common.Response;
using Inventory_Management_System.Application.Features.CustomerFeatures.Commands;
using Inventory_Management_System.Application.Features.CustomerFeatures.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management_System.API.Controllers
{
    public class CustomerController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [Route("AddOrUpdateOrDeleteCustomer")]
        [HttpPost]
        public async Task<ResponseVm<AddOrUpdateOrDeleteCustomerResponseVM>> AddOrUpdateOrDeleteCustomer([FromBody] AddOrUpdateOrDeleteCustomer command, CancellationToken cancellationToken)
        {
            var responseVm = await _mediator.Send(command);
            return responseVm;
        }

        [Authorize]
        [Route("GetAllCustomers")]
        [HttpGet]
        public async Task<ResponseVm<List<GetAllCustomersQueryResponseVM>>> GetAllCustomers([FromQuery] GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            var responseVm = await _mediator.Send(request);
            return responseVm;
        }

        [Authorize]
        [Route("GetCustomerById")]
        [HttpGet]
        public async Task<ResponseVm<GetCustomerByIdQueryResponseVM>> GetCustomerById([FromQuery] GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var responseVm = await _mediator.Send(request);
            return responseVm;
        }
    }
}
