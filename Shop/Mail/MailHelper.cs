using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;
using System.Net;

namespace Shop.Mail
{
    public class MailHelper
    {
        public void SendEmail(string address, string subject, string message)
        {
            // Đọc thông tin đăng nhập từ biến môi trường
            string email = Environment.GetEnvironmentVariable("diepdiep181223@gmail.com");
            string password = Environment.GetEnvironmentVariable("0981484579aB");

            var loginInfo = new NetworkCredential(email, password);
            var msg = new MailMessage();
            var smtpClient = new SmtpClient("smtp.gmail.com", 465);

            msg.From = new MailAddress(email);
            msg.To.Add(new MailAddress(address));
            msg.Subject = subject;
            msg.Body = message;
            msg.IsBodyHtml = true;

            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = loginInfo;

            try
            {
                smtpClient.Send(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught while sending the email: {0}", ex.ToString());
            }
        }
    }
}