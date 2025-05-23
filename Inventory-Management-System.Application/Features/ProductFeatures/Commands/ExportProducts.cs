using Inventory_Management_System.Application.Common.Response;
using Inventory_Management_System.Application.Features.ProductSupplierFeatures.Commands;
using Inventory_Management_System.Application.Interfaces;
using Inventory_Management_System.Domain.Common;
using Inventory_Management_System.Infrastructure.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_System.Application.Features.ProductFeatures.Commands
{
    public class ExportProductsResponseVM 
    {
        public byte[] File { get; set; } = Array.Empty<byte>();
    }
    public class ExportProductDetailsResponseVM
    {
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Price Per Quantity")]
        public decimal PricePerQuantity { get; set; }

        [Display(Name = "Quantity")]
        public decimal Quantity { get; set; }

        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Total Quantities")]
        public decimal TotalQuantities { get; set; }

        [Display(Name = "Stock Keeping Unit")]
        public string? StockKeepingUnit { get; set; }

        [Display(Name = "Category")]
        public string? Category { get; set; }

        [Display(Name = "Supplier Name")]
        public string? Supplier { get; set; }

        [Display(Name = "Is Deleted")]
        public string? IsDeleted { get; set; }

        [Display(Name = "Created At")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Last Modified At")]
        public DateTime? LastModifiedAt { get; set; }

        [Display(Name = "Last Modified By")]
        public string? LastModifiedBy { get; set; }

        [Display(Name = "Deleted At")]
        public DateTime? DeletedAt { get; set; }

    }
    public class ExportProducts: IRequest<ResponseVm<ExportProductsResponseVM>>
    {
    }
    public class ExportProductsHandler : ResponseWrapper<ExportProductsResponseVM>, IRequestHandler<ExportProducts, ResponseVm<ExportProductsResponseVM>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<ExportProductsHandler> _logger;
        private readonly IExportExcelService _exportExcelService;


        public ExportProductsHandler(IApplicationDbContext context, ILogger<ExportProductsHandler> logger, IExportExcelService exportExcelService)
        {
            _context = context;
            _logger = logger;
            _exportExcelService = exportExcelService;
        }

        public async Task<ResponseVm<ExportProductsResponseVM>> Handle(ExportProducts request, CancellationToken cancellationToken)
        {
            try
            {
                var getProducts = await (from product in _context.Products

                                         join supplier in _context.ProductSuppliers
                                         on product.SupplierId equals supplier.Id into suppliers
                                         from supplier in suppliers.DefaultIfEmpty() 

                                         join category in _context.ProductCategories
                                         on product.CategoryId equals category.Id into categories
                                         from category in categories.DefaultIfEmpty() 

                                         join stockKeepingUnit in _context.StockKeepingUnits
                                         on product.StockKeepingUnitId equals stockKeepingUnit.Id into stockKeepingUnits
                                         from stockKeepingUnit in stockKeepingUnits.DefaultIfEmpty() 

                                         select new ExportProductDetailsResponseVM()
                                         {
                                             Name = product.Name,
                                             Description = product.Description,

                                             Category = category != null ? category.Name : null, 
                                             Supplier = supplier != null ? supplier.DistributorName : null,
                                             StockKeepingUnit = stockKeepingUnit != null ? stockKeepingUnit.Name : null, 

                                             PricePerQuantity = product.PricePerQuantity,
                                             Quantity = product.Quantity,
                                             TotalPrice = product.TotalPrice,
                                             TotalQuantities = product.TotalQuantities,

                                             CreatedBy = product.CreatedBy,
                                             CreatedAt = product.CreatedAt,
                                             LastModifiedBy = product.LastModifiedBy,
                                             LastModifiedAt = product.LastModifiedAt,
                                             IsDeleted = product.IsDeleted ? "Yes" : "No",
                                             DeletedAt = product.DeletedAt

                                         }).ToListAsync(cancellationToken);


                var file = _exportExcelService.CreateFile(getProducts);

                if (file == null || file.Length == 0)
                {
                    return Return400(new ResponseMessage() { Description = "Failed to generate Excel file.", Name = "Error", Status = ResponseStatus.Error});
                }

                // Return Response...
                var responseData = new ExportProductsResponseVM()
                {
                    File = file 
                };
                return Return200WithData(responseData, new ResponseMessage() { Description = "Products are downloaded successfully.", Name = "Success", Status = ResponseStatus.Success });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Logged.Error : ExportProductsHandler {0}", ex.Message);
                return Return400(new ResponseMessage() { Description = "Failed to download the products", Name = "Error", Status = ResponseStatus.Error });
            }
        }
    }
}
