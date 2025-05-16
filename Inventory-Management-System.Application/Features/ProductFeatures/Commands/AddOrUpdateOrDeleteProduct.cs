using Inventory_Management_System.Application.Common.Response;
using Inventory_Management_System.Domain;
using Inventory_Management_System.Infrastructure.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management_System.Application.Features.ProductFeatures.Commands
{
    #region ResponseVm
    public class AddOrUpdateOrDeleteProductVm
    {
        public string ActionType { get; set; }
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal PricePerQuantity { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalQuantities { get; set; }
        public Guid? StockKeepingUnitId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? SupplierId { get; set; }
        public bool IsDeleted { get; set; }
    }
    #endregion

    public class AddOrUpdateOrDeleteProduct : IRequest<ResponseVm<AddOrUpdateOrDeleteProductVm>>
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal PricePerQuantity { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalQuantities { get; set; }
        public Guid? StockKeepingUnitId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? SupplierId { get; set; }
        public bool IsDeleted { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageName { get; set; }
        public string? ImageType { get; set; }

    }

    public class AddOrUpdateOrDeleteProductHandler : ResponseWrapper<AddOrUpdateOrDeleteProductVm>, IRequestHandler<AddOrUpdateOrDeleteProduct, ResponseVm<AddOrUpdateOrDeleteProductVm>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILogger<AddOrUpdateOrDeleteProductHandler> _logger;
        public AddOrUpdateOrDeleteProductHandler(IApplicationDbContext dbContext, ILogger<AddOrUpdateOrDeleteProductHandler> logger) 
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ResponseVm<AddOrUpdateOrDeleteProductVm>> Handle(AddOrUpdateOrDeleteProduct request, CancellationToken cancellationToken)
        {
            try
            {
                var product = new Product();

                #region Add / Update Product
                if (request.Id == null || request.Id == Guid.Empty)
                {
                    product = new Product
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = ""
                    };
                    _dbContext.Products.Add(product);
                }
                else
                {
                    product = _dbContext.Products.FirstOrDefault(x => x.Id == request.Id && !x.IsDeleted);
                    if (product != null)
                    {
                        product.LastModifiedAt = DateTime.UtcNow;
                        product.LastModifiedBy = "";
                    }
                }

                if (product != null)
                {
                    product.Name = request.Name != null ? request.Name : product.Name ;
                    product.Description = request.Description;
                    product.CategoryId = request.CategoryId;
                    product.SupplierId = request.SupplierId;
                    product.StockKeepingUnitId = request.StockKeepingUnitId;
                    product.PricePerQuantity = request.PricePerQuantity;
                    product.Quantity = request.Quantity;
                    product.TotalQuantities = request.TotalQuantities;
                    product.TotalPrice = request.TotalQuantities * request.PricePerQuantity;
                    product.IsDeleted = request.IsDeleted;
                    product.DeletedAt = request.IsDeleted ? DateTime.UtcNow : null;
                    product.ImageUrl = request.ImageUrl;
                    product.ImageName = request.ImageName;
                    product.ImageType = request.ImageType;

                    if (request.Id != null && request.Id != Guid.Empty)
                    {
                        _dbContext.Products.Update(product);
                    }
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
                #endregion

                #region Call UploadProductImageFunction to upload image

                using var client = new HttpClient();
                var requestUrl = $"http://localhost:7177/api/products/{product.Id}/upload-image"; // Replace with actual Azure Function URL when deployed

                var imageData = new
                {
                    ImageName = product.ImageName,
                    ImageType = product.ImageType,
                    ImageURL = product.ImageUrl
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(imageData), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUrl, jsonContent);

                if (!response.IsSuccessStatusCode)
                    return Return400(new ResponseMessage() { Name = "Error:UploadProductImageFunction", Description = "Failed to upload image", Status = ResponseStatus.Error }); ;

                #endregion

                #region Response Model

                var responseVM = product != null ?
                    new AddOrUpdateOrDeleteProductVm()
                    {
                        ActionType = request.Id == null || request.Id == Guid.Empty ? "added" : "updated",
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        CategoryId = product.CategoryId,
                        SupplierId = product.SupplierId,
                        StockKeepingUnitId = product.StockKeepingUnitId,
                        PricePerQuantity = product.PricePerQuantity,
                        Quantity = product.Quantity,
                        TotalPrice = product.TotalPrice,
                        IsDeleted = product.IsDeleted,
                        TotalQuantities = product.TotalQuantities,
                    } : null;

                #endregion

                return Return200WithData(responseVM, new ResponseMessage() { Name = "Success", Description = $"Product {responseVM?.ActionType} successfully.", Status = ResponseStatus.Success});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Logged.Error : AddOrUpdateOrDeleteProductHandler {0}", ex.Message);
                return Return400(new ResponseMessage() { Name = "Error", Description = ex.Message, Status = ResponseStatus.Error });
            }
        }

    }
}
