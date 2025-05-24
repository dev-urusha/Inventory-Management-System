using Inventory_Management_System.Application.Common.Response;
using Inventory_Management_System.Domain;
using Inventory_Management_System.Infrastructure.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Inventory_Management_System.Application.Interfaces;
using Inventory_Management_System.Domain.Common;

namespace Inventory_Management_System.Application.Features.CustomerFeatures.Commands
{

    #region Response Model
    public class AddOrUpdateOrDeleteCustomerResponseVM: BaseEntity
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

    #region command Model
    public class AddOrUpdateOrDeleteCustomer: IRequest<ResponseVm<AddOrUpdateOrDeleteCustomerResponseVM>>
    {
        public Guid? Id { get; set; }

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
        public bool IsDeleted { get; set; }

    }
    #endregion

    #region Handler
    public class AddOrUpdateOrDeleteCustomerHandler : ResponseWrapper<AddOrUpdateOrDeleteCustomerResponseVM>, IRequestHandler<AddOrUpdateOrDeleteCustomer, ResponseVm<AddOrUpdateOrDeleteCustomerResponseVM>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILogger<AddOrUpdateOrDeleteCustomerHandler> _logger;
        private readonly ICurrentUserService _currentUserService;


        public AddOrUpdateOrDeleteCustomerHandler(IApplicationDbContext dbContext, ILogger<AddOrUpdateOrDeleteCustomerHandler> logger, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<ResponseVm<AddOrUpdateOrDeleteCustomerResponseVM>> Handle(AddOrUpdateOrDeleteCustomer command, CancellationToken cancellationToken)
        {
            try
            {
                var customer = new Customer();

                var statuses = _dbContext.Statuses.Where(x => x.Type == "Customers" && !x.IsDeleted).AsQueryable();
                var status = statuses.Where(x => x.Id == command.StatusId).FirstOrDefault();

                #region Add / Update Customer
                if (command.Id == null || command.Id == Guid.Empty)
                {
                    customer = new Customer
                    {
                        Id = Guid.NewGuid(),
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = _currentUserService.UserName,
                        IsDeleted = false,
                    };
                    _dbContext.Customers.Add(customer);
                }
                else
                {
                    customer = _dbContext.Customers.FirstOrDefault(x => x.Id == command.Id && !x.IsDeleted);
                    if (customer != null) 
                    {
                        customer.LastModifiedAt = DateTime.UtcNow;
                        customer.LastModifiedBy = _currentUserService.UserName;
                        customer.IsDeleted = command.IsDeleted ? true : false;
                        customer.DeletedAt = command.IsDeleted ? DateTime.UtcNow : null;
                    }
                    else
                    {
                        Return400WithData((customer, new ResponseMessage() { Description = "Customer does not exist or has been marked as inactive in the system.", Name = "Success", Status = ResponseStatus.Success }));
                    }
                   
                }

                if (customer != null)
                {
                    customer.FirstName = command.FirstName != null ? command.FirstName : customer.FirstName;
                    customer.MiddleName = command.MiddleName != null ? command.MiddleName : customer.MiddleName;
                    customer.LastName = command.LastName != null ? command.LastName : customer.LastName;
                    customer.Email = command.Email != null ? command.Email : customer.Email;
                    customer.Mobile = command.Mobile != null ? command.Mobile : customer.Mobile;
                    customer.AddressLineOne = command.AddressLineOne != null ? command.AddressLineOne : customer.AddressLineOne;
                    customer.AddressLineTwo = command.AddressLineTwo != null ? command.AddressLineTwo : customer.AddressLineTwo;
                    customer.City = command.City != null ? command.City : customer.City;
                    customer.State = command.State != null ? command.State : customer.State;
                    customer.ZipCode = command.ZipCode != null ? command.ZipCode : customer.ZipCode;
                    customer.Country = command.Country != null ? command.Country : customer.Country;
                    customer.StatusId = status?.Id;

                    if (command.Id != null && command.Id != Guid.Empty)
                    {
                        _dbContext.Customers.Update(customer);
                    }
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
                #endregion

                #region Response Model

                var responseVM = customer != null ?
                new AddOrUpdateOrDeleteCustomerResponseVM()
                {
                        Id = customer.Id,
                        FullName = string.Format("{0} {1} {2}", customer.FirstName, customer.MiddleName, customer.LastName),
                        FirstName = customer.FirstName,
                        MiddleName = customer.MiddleName,
                        LastName = customer.LastName,
                        Email = customer.Email,
                        Mobile = customer.Mobile,
                        AddressLineOne = customer.AddressLineOne,
                        AddressLineTwo = customer.AddressLineTwo,
                        City = customer.City,
                        State = customer.State,
                        ZipCode = customer.ZipCode,
                        Country = customer.Country,
                        StatusId = status?.Id,
                        StatusName = status?.Name,
                        CreatedBy = customer.CreatedBy,
                        CreatedAt = customer.CreatedAt,
                        LastModifiedBy = customer.LastModifiedBy,
                        LastModifiedAt = customer.LastModifiedAt,
                        IsDeleted = customer.IsDeleted ? "Yes" : "No",
                        DeletedAt = customer.DeletedAt

                } : null;

                #endregion

                return Return200WithData(responseVM, new ResponseMessage() { Description = "Customer details saved successfully.", Name = "Success", Status = ResponseStatus.Success });

            }
            catch (Exception ex)
            {
                _logger.LogError($"Logged.Error : AddOrUpdateOrDeleteCustomerHandler {0}", ex.Message);
                return Return400(new ResponseMessage() { Description = "Failed to save customer details", Name = "Error", Status = ResponseStatus.Error });
            }
        }
    }
    #endregion
}
