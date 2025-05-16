using System;
using System.Net.Mail;
using System.Text;
using Castle.Core.Smtp;
using Inventory_Management_System.Application.Common.Constants;
using Inventory_Management_System.Application.Services;
using Inventory_Management_System.Domain;
using Inventory_Management_System.Infrastructure.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;

namespace Inventory_Management_System.FunctionApp
{
    public class UpdateProductStockFunction
    {
        private readonly ILogger _logger;
        private readonly IApplicationDbContext _context;
        private readonly SendEmailService _emailSender;

        public UpdateProductStockFunction(ILoggerFactory loggerFactory, IApplicationDbContext context, SendEmailService emailSender)
        {
            _logger = loggerFactory.CreateLogger<UpdateProductStockFunction>();
            _context = context;
            _emailSender = emailSender;
        }

        [Function("UpdateProductStockFunction")]
        public async Task Run([TimerTrigger("0 0 9 * * Mon,Thu")] TimerInfo myTimer)
        {
            try
            {
                _logger.LogInformation("Product Stock Alert Scheduler Function started at: {time}", DateTime.UtcNow);

                IQueryable<Product> products = _context.Products.Where(x => !x.IsDeleted && x.TotalQuantities <= 5);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(CommonConstants.UsersOtp_SenderAddress),
                    Subject = $"Stock Alert: Products are running low. Order soon!",
                    IsBodyHtml = true,
                };

                // Construct the HTML table
                var sb = new StringBuilder();
                sb.Append("<html><body>");
                sb.Append("<h3>Stock Alert: The following products are running low!</h3>");
                sb.Append("<table border='1' cellpadding='5' cellspacing='0' style='border-collapse: collapse; width: 100%;'>");
                sb.Append("<tr style='background-color: #f2f2f2;'>");
                sb.Append("<th style='text-align: left; padding: 8px;'>Product Name</th>");
                sb.Append("<th style='text-align: left; padding: 8px;'>Remaining Quantity</th>");
                sb.Append("</tr>");


                foreach (var product in products)
                {
                    // Check if product is about to end
                    if (product.HasLimitedStock != true)
                    {
                        product.HasLimitedStock = true;
                        _logger.LogInformation($"Stock Alert: {product.Name} is running low. Order soon!");

                        sb.Append("<tr>");
                        sb.Append($"<td style='padding: 8px;'>{product.Name}</td>");
                        sb.Append($"<td style='padding: 8px;'>{product.TotalQuantities}</td>");
                        sb.Append("</tr>");
                    }
                }

                sb.Append("</table>");
                sb.Append("</body></html>");

                // Assign the formatted HTML to the email body
                mailMessage.Body = sb.ToString();

                 _emailSender.Send(mailMessage);

                await _context.SaveChangesAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Project Status Scheduler Function: {ex.Message}");
            }

        }
    }
}
