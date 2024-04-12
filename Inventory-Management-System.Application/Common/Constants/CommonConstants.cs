using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management_System.Application.Common.Constants
{
    public class CommonConstants
    {
        public const string UsersOtp_EmailTemplate = "<div>Hi {0},</div><div><br> </div><div>We received a request to reset the password associated with your account." +
            " To proceed with the password reset, please use the following One-Time Password (OTP) within the next <b>{1}</b>" +
            " IST / <b>{2}</b> UTC time.</div><div><br> </div><div>OTP: <b>{3}</b></div><div><br>" +
            " </div><div>If it expires before you complete the process, you can initiate a new password reset request by visiting our <b>Forgot Password</b> page." +
            " </div><div><br> </div><div>Thank you for your prompt attention to this matter.</div><div><br> </div><div>Best regards,</div><div><br> </div><div>dev_Urusha | urushaaazmi96@gmail.com</div><div><br> </div> ";

        public const string UsersOtp_EmailSubject = "Forgot Password - One-Time Password (OTP)";

        public const string UsersOtp_SenderAddress = "urusha****shaikh@gmail.com";

    }
}
