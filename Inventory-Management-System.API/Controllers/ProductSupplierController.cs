using Inventory_Management_System.Application.Common.Response;
using Inventory_Management_System.Application.Features.ProductFeatures.Commands;
using Inventory_Management_System.Application.Features.ProductFeatures.Queries;
using Inventory_Management_System.Application.Features.ProductSupplierFeatures.Commands;
using Inventory_Management_System.Application.Features.ProductSupplierFeatures.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management_System.API.Controllers
{
    public class ProductSupplierController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        public ProductSupplierController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("AddOrUpdateProductSupplier")]
        [HttpPost]
        public async Task<ResponseVm<AddOrUpdateOrDeleteProductSupplierResponseVM>> AddOrUpdateProductSupplier([FromBody] AddOrUpdateOrDeleteProductSupplier request, CancellationToken cancellationToken)
        {
            var responseVm = await _mediator.Send(request);
            return responseVm;
        }

        [Route("GetAllProductSuppliers")]
        [HttpGet]
        public async Task<ResponseVm<List<GetAllProductSupplierResponseVM>>> GetAllProductSuppliers([FromQuery] GetAllProductSupplierQuery request, CancellationToken cancellationToken)
        {
            var responseVm = await _mediator.Send(request);
            return responseVm;
        }
    }
}
