using Inventory_Management_System.Application.Common.Response;
using Inventory_Management_System.Application.Features.ProductFeatures.Queries;
using Inventory_Management_System.Infrastructure.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management_System.Application.Features.ProductSupplierFeatures.Queries
{

    #region Response Model
    public class GetAllProductSupplierResponseVM
    {
        public Guid Id { get; set; }
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
    }
    #endregion

    #region Request Model
    public class GetAllProductSupplierQuery : IRequest<ResponseVm<GetAllProductSupplierResponseVM>>
    {
    }
    #endregion

    #region Handler
    public class GetAllProductSupplierQueryHandler : ResponseWrapper<GetAllProductSupplierResponseVM>, IRequestHandler<GetAllProductSupplierQuery, ResponseVm<GetAllProductSupplierResponseVM>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetAllProductSupplierQueryHandler> _logger;

        public GetAllProductSupplierQueryHandler(IApplicationDbContext context, ILogger<GetAllProductSupplierQueryHandler> logger) 
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ResponseVm<GetAllProductSupplierResponseVM>> Handle(GetAllProductSupplierQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var responseData = await (from productSupplier in _context.ProductSuppliers.Where(x => !x.IsDeleted)
                                          select new GetAllProductSupplierResponseVM()
                                          {
                                              Id = productSupplier.Id,
                                              BrandName = productSupplier.BrandName,
                                              DistributorName = productSupplier.DistributorName,
                                              Email = productSupplier.Email,
                                              Phone = productSupplier.Phone,
                                              AddressLineOne = productSupplier.AddressLineOne,
                                              AddressLineTwo = productSupplier.AddressLineTwo,
                                              City = productSupplier.City,
                                              State = productSupplier.State,
                                              Country = productSupplier.Country,
                                              PostalCode = productSupplier.PostalCode,
                                          }).ToListAsync();

                return Return200WithData(responseData, new ResponseMessage() { Description = "Product suppliers data are fetched successfully.", Name = "Success", Status = ResponseStatus.Success });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Logged Error: GetAllProductSupplierQueryHandler {0}", ex.Message);
                return Return400(new ResponseMessage() { Name = "Error", Description = ex.Message, Status = ResponseStatus.Error });
            }
        }
    }
    #endregion
}
