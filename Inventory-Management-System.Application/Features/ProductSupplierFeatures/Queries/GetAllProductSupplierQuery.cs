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
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
    }
    #endregion

    #region Request Model
    public class GetAllProductSupplierQuery : IRequest<ResponseVm<List<GetAllProductSupplierResponseVM>>>
    {
    }
    #endregion

    #region Handler
    public class GetAllProductSupplierQueryHandler : ResponseWrapper<List<GetAllProductSupplierResponseVM>>, IRequestHandler<GetAllProductSupplierQuery, ResponseVm<List<GetAllProductSupplierResponseVM>>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetAllProductSupplierQueryHandler> _logger;

        public GetAllProductSupplierQueryHandler(IApplicationDbContext context, ILogger<GetAllProductSupplierQueryHandler> logger) 
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ResponseVm<List<GetAllProductSupplierResponseVM>>> Handle(GetAllProductSupplierQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var responseData = await (from productSupplier in _context.ProductSuppliers
                                          where !productSupplier.IsDeleted
                                          select new GetAllProductSupplierResponseVM()
                                          {
                                              Id = productSupplier.Id,
                                              BrandName = productSupplier.BrandName ?? string.Empty,
                                              DistributorName = productSupplier.DistributorName ?? string.Empty,
                                              Email = productSupplier.Email ?? string.Empty,
                                              Phone = productSupplier.Phone ?? 0,
                                              AddressLineOne = productSupplier.AddressLineOne ?? string.Empty,
                                              AddressLineTwo = productSupplier.AddressLineTwo ?? string.Empty,
                                              City = productSupplier.City ?? string.Empty,
                                              State = productSupplier.State ?? string.Empty,
                                              Country = productSupplier.Country ?? string.Empty,
                                              PostalCode = productSupplier.PostalCode ?? string.Empty

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
