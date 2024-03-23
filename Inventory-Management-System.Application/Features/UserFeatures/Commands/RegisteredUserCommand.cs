using Inventory_Management_System.Application.Common.Response;
using Inventory_Management_System.Domain;
using Inventory_Management_System.Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management_System.Application.Features.UserFeatures.Commands
{
    #region Response
    public class RegisteredUserVm
    {
        public string Username { get; set; }
        public string Role { get; set; }
    }
    #endregion

    #region Request
    public class RegisteredUserCommand : IRequest<ResponseVm<RegisteredUserVm>>
    {
        public Guid? Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string CurrentPassword { get; set; }
        public Guid RoleId { get; set; }
    }
    #endregion

    public class RegisteredUserCommandHandler : ResponseWrapper<RegisteredUserVm>, IRequestHandler<RegisteredUserCommand, ResponseVm<RegisteredUserVm>>
    {
        private readonly ILogger<RegisteredUserCommandHandler> _logger;
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IPasswordHasher<User> _passwordHasher;

        public RegisteredUserCommandHandler(ILogger<RegisteredUserCommandHandler> logger, IApplicationDbContext applicationDbContext, IPasswordHasher<User> passwordHasher)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<ResponseVm<RegisteredUserVm>> Handle(RegisteredUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                #region Validations
                var role = await _applicationDbContext.Roles.FirstOrDefaultAsync(x => x.Id == command.RoleId && !x.IsDeleted);
                if (role == null)
                {
                    throw new FluentValidation.ValidationException("Role does not exists in the system.");
                }
                #endregion

                User existing;
                if (!command.Id.HasValue || command.Id == Guid.Empty)
                {
                    existing = await _applicationDbContext.Users.FirstOrDefaultAsync(x => x.Username == command.Username);
                    if (existing != null)
                    {
                        if (existing.IsDeleted)
                        {
                            throw new FluentValidation.ValidationException(string.Concat("User already exist.", Environment.NewLine, "User is Inactive."));

                        }
                        throw new FluentValidation.ValidationException("User already exist.");

                    }
                    existing = new User()
                    {
                        Id = Guid.NewGuid(),
                        Username = command.Username,
                        RoleId = role.Id,
                        StatusId = _applicationDbContext.Statuses.FirstOrDefault(x => x.Name == "Active" && x.Type == "Users" && !x.IsDeleted)?.Id,
                        Password = command.Password,
                    };
                    existing.Password = HashPassword(existing, existing.Password); // Hash the password               
                    _applicationDbContext.Users.Add(existing);
                    await _applicationDbContext.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    existing = await _applicationDbContext.Users.FirstOrDefaultAsync(x => x.Id == command.Id);
                    if (existing == null)
                    {
                        throw new FluentValidation.ValidationException("No record found with the given Id.");
                    }

                    var verifyPasscode = VerifyHashedPassword(existing, existing.Password, command.CurrentPassword);
                    if (verifyPasscode != PasswordVerificationResult.Success)
                    {
                        throw new FluentValidation.ValidationException("Invalid Passcode.");
                    }

                    existing.Password = HashPassword(existing, command.Password); // Hash the password               
                    _applicationDbContext.Users.Update(existing);
                }
                await _applicationDbContext.SaveChangesAsync(cancellationToken);

                var responseVm = new RegisteredUserVm()
                {
                    Username = command.Username,
                    Role = ""
                };
                return Return200WithData(responseVm, new ResponseMessage() { Name = "Succes", Description = "Requested user registered successfully.", Status = ResponseStatus.Success });
            }
            catch (Exception ex)
            {
                _logger.LogError($"RegisteredUserCommandHandler.Handle: Error {ex.Message}",ex);
                return Return400(new ResponseMessage() { Name = "Error", Description = "Failed to registered requested user.", Status = ResponseStatus.Success });
            }
        }

        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        {
            return _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        }
    }
}
