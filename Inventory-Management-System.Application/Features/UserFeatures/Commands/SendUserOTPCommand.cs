using Castle.Core.Smtp;
using Inventory_Management_System.Application.Common.Constants;
using Inventory_Management_System.Application.Common.Response;
using Inventory_Management_System.Application.Services;
using Inventory_Management_System.Domain;
using Inventory_Management_System.Infrastructure.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management_System.Application.Features.UserFeatures.Commands
{
    #region Response 
    public class SendUserOTPVm
    {
    }
    #endregion

    public class SendUserOTPCommand : IRequest<ResponseVm<SendUserOTPVm>>
    {
        public string Username { get; set; }
    }

    public class SendUserOTPCommandHandler : ResponseWrapper<SendUserOTPVm>, IRequestHandler<SendUserOTPCommand, ResponseVm<SendUserOTPVm>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly SendEmailService _emailSender;
        public SendUserOTPCommandHandler(IApplicationDbContext applicationDbContext, SendEmailService emailSender) 
        {
            _applicationDbContext = applicationDbContext;
            _emailSender = emailSender;
        }

        public async Task<ResponseVm<SendUserOTPVm>> Handle(SendUserOTPCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var responseVm = new SendUserOTPVm() { };
                #region Validations
                // Username can't be pass as null in payload........
                if (command.Username == null)
                {
                    throw new FluentValidation.ValidationException($"Username is required.");
                }

                // Check username whether exists in the system or not.........
                var validateUsername = await _applicationDbContext.Users.FirstOrDefaultAsync(x => x.Username == command.Username && !x.IsDeleted);
                if (validateUsername == null)
                {
                    throw new FluentValidation.ValidationException($"Username {command.Username} not registered.");
                }
                #endregion

                //Save OTP...
                var saveUserOtp = new UsersOTP()
                {
                    Id = new Guid(),
                    GeneratedOTP = (new Random().Next(1000, 9999)).ToString(),
                    ExpirationTime = DateTime.UtcNow.AddMinutes(15),
                    UserId = validateUsername.Id,
                    CreatedAt = DateTime.UtcNow
                };

                _applicationDbContext.UsersOTPs.Add(saveUserOtp);
                _applicationDbContext.SaveChangesAsync(cancellationToken);

                SendOtpByEmail(validateUsername.Username, saveUserOtp.GeneratedOTP, TimeZoneInfo.ConvertTimeFromUtc((DateTime)saveUserOtp.ExpirationTime, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")), saveUserOtp.ExpirationTime);

                return Return200WithData(responseVm, new ResponseMessage() { Name = "", Description = "", Status= ResponseStatus.Success });
            }
            catch (Exception ex)
            {
                return Return400(new ResponseMessage() { Name = "", Description = ex.Message, Status = ResponseStatus.Error });
            }
        }

        public void SendOtpByEmail(string email, string otp, DateTime IndiaStandardTime, DateTime UTCDateTimeVar)
        {
            _emailSender.Send(email, otp, IndiaStandardTime.Date.ToString(), UTCDateTimeVar.Date.ToString());
        }
    }
}
