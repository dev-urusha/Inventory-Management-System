using Inventory_Management_System.Application.Common.Response;
using Inventory_Management_System.Application.Features.ProductFeatures.Commands;
using Inventory_Management_System.Domain;
using Inventory_Management_System.Infrastructure.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management_System.Application.Features.ProductFeatures.Queries
{
    #region Response Model
    public class GetAllProductsResponseVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal PricePerQuantity { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalQuantities { get; set; }

        public Guid? StockKeepingUnitId { get; set; }
        public string? StockKeepingUnitName { get; set; }

        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public Guid? SupplierId { get; set; }
        public string? SupplierName { get; set; }
    }
    #endregion

    #region Request Model
    public class GetAllProductsQuery : IRequest<ResponseVm<GetAllProductsResponseVM>>
    {
    }
    #endregion

    #region Handler
    public class GetAllProductsQueryHandler : ResponseWrapper<GetAllProductsResponseVM>, IRequestHandler<GetAllProductsQuery, ResponseVm<GetAllProductsResponseVM>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetAllProductsQueryHandler> _logger;

        public GetAllProductsQueryHandler(IApplicationDbContext context, ILogger<GetAllProductsQueryHandler> logger) 
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ResponseVm<GetAllProductsResponseVM>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var responseData = await (from product in _context.Products.Where(x => !x.IsDeleted)
                                      select new GetAllProductsResponseVM() { 
                                          Id = product.Id,
                                          Name = product.Name,
                                          Description = product.Description,
                                          Quantity = product.Quantity,
                                          PricePerQuantity = product.PricePerQuantity,
                                          TotalPrice = product.TotalPrice,
                                          TotalQuantities = product.TotalQuantities,

                                          StockKeepingUnitId = product.StockKeepingUnitId,
                                          StockKeepingUnitName = product.StockKeepingUnitId != null ? _context.StockKeepingUnits.FirstOrDefault(x => x.Id == product.StockKeepingUnitId).Name : null,

                                          CategoryId = product.CategoryId,
                                          CategoryName = product.CategoryId != null ? _context.ProductCategories.FirstOrDefault(x => x.Id == product.CategoryId).Name : null,

                                          SupplierId = product.SupplierId,
                                          SupplierName = product.SupplierId != null ? _context.ProductSuppliers.FirstOrDefault(x => x.Id == product.SupplierId).DistributorName : null,

                                      }).ToListAsync();

                return Return200WithData(responseData, new ResponseMessage() { Description = "Products data are fetched successfully.", Name = "Success", Status = ResponseStatus.Success });

            }
            catch (Exception ex) 
            {
                _logger.LogError($"Logged.Error : GetAllProductsQueryHandler {0}", ex.Message);
                return Return400(new ResponseMessage() { Description = "Failed to fetch products data", Name = "Error", Status = ResponseStatus.Error });
            }
        }
    }
    #endregion
}
