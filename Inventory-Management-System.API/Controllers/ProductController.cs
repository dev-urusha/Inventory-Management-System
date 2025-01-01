using Inventory_Management_System.Application.Common.Response;
using Inventory_Management_System.Application.Features.ProductFeatures.Commands;
using Inventory_Management_System.Application.Features.ProductFeatures.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management_System.API.Controllers
{
    public class ProductController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        public ProductController(IMediator mediator) 
        {
            _mediator = mediator;
        }

        [Route("AddOrUpdateProduct")]
        [HttpPost]
        public async Task<ResponseVm<AddOrUpdateOrDeleteProductVm>> AddOrUpdateProduct([FromBody] AddOrUpdateOrDeleteProduct request, CancellationToken cancellationToken)
        {
            var responseVm = await _mediator.Send(request);
            return responseVm;
        }

        [Route("GetAllProducts")]
        [HttpGet]
        public async Task<ResponseVm<List<GetAllProductsResponseVM>>> GetAllProducts([FromQuery] GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var responseVm = await _mediator.Send(request);
            return responseVm;
        }
    }
}
