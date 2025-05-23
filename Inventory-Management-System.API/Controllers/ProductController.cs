﻿using Inventory_Management_System.Application.Common.Response;
using Inventory_Management_System.Application.Features.ProductFeatures.Commands;
using Inventory_Management_System.Application.Features.ProductFeatures.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [Route("AddOrUpdateProduct")]
        [HttpPost]
        public async Task<ResponseVm<AddOrUpdateOrDeleteProductVm>> AddOrUpdateProduct([FromBody] AddOrUpdateOrDeleteProduct request, CancellationToken cancellationToken)
        {
            var responseVm = await _mediator.Send(request);
            return responseVm;
        }

        [Authorize]
        [Route("GetAllProducts")]
        [HttpGet]
        public async Task<ResponseVm<List<GetAllProductsResponseVM>>> GetAllProducts([FromQuery] GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var responseVm = await _mediator.Send(request);
            return responseVm;
        }

        [Authorize]
        [Route("ExportProducts")]
        [HttpGet]
        public async Task<IActionResult> ExportProducts([FromQuery] ExportProducts request, CancellationToken ct)
        {
            var responseVm = await _mediator.Send(request);
            var fileName = "Products" + "_" + DateTime.UtcNow + ".xlsx";
            return File(responseVm.Data.File, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [Authorize]
        [Route("GetProductById")]
        [HttpGet]
        public async Task<ResponseVm<GetProductByIdQueryResponseVM>> GetProductById([FromQuery] GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var responseVm = await _mediator.Send(request);
            return responseVm;
        }

    }
}
