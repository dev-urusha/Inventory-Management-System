using Inventory_Management_System.Application.Common.Response;
using Inventory_Management_System.Application.Features.ProductFeatures.Commands;
using Inventory_Management_System.Domain;
using Inventory_Management_System.Infrastructure.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management_System.Application.Features.ProductSupplierFeatures.Commands
{

    #region Response Class
    public class AddOrUpdateOrDeleteProductSupplierResponseVM 
    {
        public Guid? Id { get; set; }
        public string? DistributorName { get; set; }
        public string? BrandName { get; set; }
        public string? Email { get; set; }
        public int? Phone { get; set; }
        public string? AddressLineOne { get; set; }
        public string? AddressLineTwo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public bool IsDeleted { get; set; }
        public string ActionType { get; set; }
    }
    #endregion

    #region Request Class
    public class AddOrUpdateOrDeleteProductSupplier : IRequest<ResponseVm<AddOrUpdateOrDeleteProductSupplierResponseVM>>
    {
        public Guid? Id { get; set; }
        public string? DistributorName { get; set; }
        public string? BrandName { get; set; }
        public string? Email { get; set; }
        public int? Phone { get; set; }
        public string? AddressLineOne { get; set; }
        public string? AddressLineTwo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public bool IsDeleted { get; set; }

    }
    #endregion

    #region Business Logic Class
    public class AddOrUpdateOrDeleteProductSupplierHandler : ResponseWrapper<AddOrUpdateOrDeleteProductSupplierResponseVM>, IRequestHandler<AddOrUpdateOrDeleteProductSupplier, ResponseVm<AddOrUpdateOrDeleteProductSupplierResponseVM>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILogger<AddOrUpdateOrDeleteProductHandler> _logger;
        public AddOrUpdateOrDeleteProductSupplierHandler(IApplicationDbContext dbContext, ILogger<AddOrUpdateOrDeleteProductHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ResponseVm<AddOrUpdateOrDeleteProductSupplierResponseVM>> Handle(AddOrUpdateOrDeleteProductSupplier request, CancellationToken cancellationToken)
        {
            try
            {
                var productSupplier = new ProductSupplier();

                #region Add / Update Product Supplier
                if (request.Id == null || request.Id == Guid.Empty)
                {
                    productSupplier = new ProductSupplier
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = ""
                    };
                    _dbContext.ProductSuppliers.Add(productSupplier);
                }
                else
                {
                    productSupplier = _dbContext.ProductSuppliers.FirstOrDefault(x => x.Id == request.Id && !x.IsDeleted);
                    if (productSupplier != null)
                    {
                        productSupplier.LastModifiedAt = DateTime.UtcNow;
                        productSupplier.LastModifiedBy = "";
                    }
                }

                if (productSupplier != null)
                {
                    productSupplier.BrandName = request.BrandName;
                    productSupplier.DistributorName = request.DistributorName;
                    productSupplier.Email = request.Email;
                    productSupplier.Phone = request.Phone;
                    productSupplier.AddressLineOne = request.AddressLineOne;
                    productSupplier.AddressLineTwo = request.AddressLineTwo;
                    productSupplier.Country = request.Country;
                    productSupplier.State = request.State;
                    productSupplier.City = request.City;
                    productSupplier.PostalCode = request.PostalCode;
                    productSupplier.IsDeleted = request.IsDeleted;
                    productSupplier.DeletedAt = request.IsDeleted ? DateTime.UtcNow : null;

                    if (request.Id != null && request.Id != Guid.Empty)
                    {
                        _dbContext.ProductSuppliers.Update(productSupplier);
                    }
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
                #endregion

                #region Response Model

                var responseVM = productSupplier != null ?
                    new AddOrUpdateOrDeleteProductSupplierResponseVM()
                    {
                        ActionType = request.Id == null || request.Id == Guid.Empty ? "added" : "updated",
                        Id = productSupplier.Id,
                        BrandName = productSupplier.BrandName,
                        DistributorName = productSupplier.DistributorName,
                        Email = productSupplier.Email,
                        Phone = productSupplier.Phone,
                        AddressLineOne = productSupplier.AddressLineOne,
                        AddressLineTwo = productSupplier.AddressLineTwo,
                        Country = productSupplier.Country,
                        State = productSupplier.State,
                        IsDeleted = productSupplier.IsDeleted,
                        City = productSupplier.City,
                        PostalCode = productSupplier.PostalCode
                    } : null;

                #endregion

                return Return200WithData(responseVM, new ResponseMessage() { Name = "Success", Description = $"Product Supplier {responseVM?.ActionType} successfully.", Status = ResponseStatus.Success });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Logged.Error : AddOrUpdateOrDeleteProductSupplierHandler {0}", ex.Message);
                return Return400(new ResponseMessage() { Name = "Error", Description = ex.Message, Status = ResponseStatus.Error });
            }
        }
    }
    #endregion
}
