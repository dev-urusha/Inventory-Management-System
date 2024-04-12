using Castle.Core.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Inventory_Management_System.Application.Common.Constants;

namespace Inventory_Management_System.Application.Services
{
    public class SendEmailService : IEmailSender
    {
        private readonly SmtpClient _smtpClient;

        public SendEmailService()
        {
            _smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(CommonConstants.UsersOtp_SenderAddress, "Password"),
                EnableSsl = true
            };
        }
        public void Send(string email, string otp, string IndiaStandardTime, string UTCDateTimeVar)
        {
            var message = new MailMessage(CommonConstants.UsersOtp_SenderAddress, email, CommonConstants.UsersOtp_EmailSubject, string.Format(CommonConstants.UsersOtp_EmailTemplate, email, IndiaStandardTime, UTCDateTimeVar, otp))
            {
                IsBodyHtml = true
            };

            _smtpClient.SendMailAsync(message);
        }

        public void Send(MailMessage receiverAddress)
        {
            throw new NotSupportedException();
        }

        public void Send(IEnumerable<MailMessage> receiverAddresses)
        {
            throw new NotImplementedException();
        }

    }
}
