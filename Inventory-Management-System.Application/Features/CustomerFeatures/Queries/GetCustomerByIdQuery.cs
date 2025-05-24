using Inventory_Management_System.Application.Common.Response;
using Inventory_Management_System.Domain.Common;
using Inventory_Management_System.Infrastructure.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management_System.Application.Features.CustomerFeatures.Queries
{
    #region Response Model
    public class GetCustomerByIdQueryResponseVM : BaseEntity
    {
        public Guid? Id { get; set; }

        public string FullName { get; set; }

        public string FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string LastName { get; set; }

        public string Mobile { get; set; }

        public string? Email { get; set; }

        public string AddressLineOne { get; set; }

        public string? AddressLineTwo { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Country { get; set; }

        public Int32? ZipCode { get; set; }

        public Guid? StatusId { get; set; }
        public string? StatusName { get; set; }
        public string IsDeleted { get; set; }
    }
    #endregion

    #region Request Model
    public class GetCustomerByIdQuery : IRequest<ResponseVm<GetCustomerByIdQueryResponseVM>>
    {
        public Guid Id { get; set; }
    }
    #endregion

    #region Handler
    public class GetCustomerByIdQueryHandler : ResponseWrapper<GetCustomerByIdQueryResponseVM>, IRequestHandler<GetCustomerByIdQuery, ResponseVm<GetCustomerByIdQueryResponseVM>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetCustomerByIdQueryHandler> _logger;

        public GetCustomerByIdQueryHandler(IApplicationDbContext context, ILogger<GetCustomerByIdQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ResponseVm<GetCustomerByIdQueryResponseVM>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var responseData = await (from customer in _context.Customers
                                          join status in _context.Statuses on customer.StatusId equals status.Id
                                          where !customer.IsDeleted && customer.Id == request.Id

                                          select new GetCustomerByIdQueryResponseVM()
                                          {
                                              Id = customer.Id,
                                              FirstName = customer.FirstName,
                                              LastName = customer.LastName,
                                              MiddleName = customer.MiddleName,
                                              FullName = string.Format("{0} {1} {2}", customer.FirstName, customer.MiddleName, customer.LastName),
                                              Mobile = customer.Mobile,
                                              Email = customer.Email,

                                              AddressLineOne = customer.AddressLineOne,
                                              AddressLineTwo = customer.AddressLineTwo,
                                              City = customer.City,
                                              State = customer.State,
                                              ZipCode = customer.ZipCode,
                                              Country = customer.Country,

                                              StatusId = status.Id,
                                              StatusName = status.Name,

                                              CreatedBy = customer.CreatedBy,
                                              CreatedAt = customer.CreatedAt,
                                              LastModifiedBy = customer.LastModifiedBy,
                                              LastModifiedAt = customer.LastModifiedAt,
                                              IsDeleted = customer.IsDeleted ? "Yes" : "No",
                                              DeletedAt = customer.DeletedAt

                                          }).FirstOrDefaultAsync();

                return Return200WithData(responseData, new ResponseMessage() { Description = "Customer details fetched successfully.", Name = "Success", Status = ResponseStatus.Success });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Logged.Error : GetCustomerByIdQueryHandler {0}", ex.Message);
                return Return400(new ResponseMessage() { Description = "Failed to fetch customer details", Name = "Error", Status = ResponseStatus.Error });

            }
        }

    }
    #endregion

}
