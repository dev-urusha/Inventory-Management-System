using Inventory_Management_System.Application.Common.Response;
using Inventory_Management_System.Domain;
using Inventory_Management_System.Infrastructure.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management_System.Application.Features.UserFeatures.Commands
{
    #region Response
    public class ForgotPasswordResponseVM
    {
        public Guid? UsersId { get; set; }
        public string GeneratedOTP { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public string Username { get; set; }
    }
    #endregion

    public class ForgotPasswordCommand : IRequest<ResponseVm<ForgotPasswordResponseVM>>
    {
        public string GeneratedOTP { get; set; }
        public string Username { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class ForgotPasswordCommandHandler : ResponseWrapper<ForgotPasswordResponseVM>, IRequestHandler<ForgotPasswordCommand, ResponseVm<ForgotPasswordResponseVM>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        public ForgotPasswordCommandHandler(IApplicationDbContext applicationDbContext, IPasswordHasher<User> passwordHasher)
        {
            _applicationDbContext = applicationDbContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<ResponseVm<ForgotPasswordResponseVM>> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            try
            {
                #region Validations

                // Validate User
                var existingUser = _applicationDbContext.GetDbSet<User>().FirstOrDefault(x => x.Username == command.Username);
                if (existingUser == null)
                {
                    throw new FluentValidation.ValidationException("No record found with the given user Id.");
                }

                // Validate OTP
                var generatedOTP = await _applicationDbContext.UsersOTPs.Where(u => u.UserId == existingUser.Id && u.GeneratedOTP == command.GeneratedOTP).FirstOrDefaultAsync();
                if (generatedOTP == null)
                {
                    throw new FluentValidation.ValidationException("Invalid OTP.");
                }

                if (generatedOTP.ExpirationTime < DateTime.UtcNow && !existingUser.IsDeleted)
                {
                    throw new FluentValidation.ValidationException("OTP expired.");
                }

                if (command.NewPassword !=  command.ConfirmPassword)
                {
                    throw new FluentValidation.ValidationException("Password mismatched.");
                }

                #endregion

                command.ConfirmPassword = HashPassword(existingUser, command.ConfirmPassword); // Hash the password
                existingUser.Password = command.ConfirmPassword;
                existingUser.LastModifiedAt = DateTime.UtcNow;
                _applicationDbContext.Users.Update(existingUser);
                await _applicationDbContext.SaveChangesAsync(cancellationToken);

                var responseVm = new ForgotPasswordResponseVM()
                {
                    Username = command.Username,
                    UsersId = existingUser.Id,
                };

               return Return200WithData(responseVm, new ResponseMessage() { Description = "Password reset successfully.", Name = "", Status = ResponseStatus.Success});
            }
            catch(Exception ex)
            {
               return Return400(new ResponseMessage() { Description = ex.Message, Name = "Failed to reset the password.", Status = ResponseStatus.Error });
            }
        }

        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }
    }
}
