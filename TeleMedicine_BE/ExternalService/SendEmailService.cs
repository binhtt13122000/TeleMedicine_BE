using Infrastructure.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static TeleMedicine_BE.Utils.Constants;

namespace TeleMedicine_BE.ExternalService
{
    public interface ISendEmailService
    {
        public Task<bool> SendEmail(EmailForm model);
    }
    public class SendEmailService : ISendEmailService
    {
        private IConfiguration _configuration;

        public SendEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmail(EmailForm model)
        {
            using var smtp = new SmtpClient();
            try
            {
                MimeMessage email = new MimeMessage();
                email.From.Add(new MailboxAddress("System TeleMedicine", "mailclone1007@gmail.com"));
                System.Diagnostics.Debug.WriteLine("TO:" + model.ToEmail);
                email.To.Add(MailboxAddress.Parse(model.ToEmail));
                email.Subject = model.Subject;
                email.Body = new TextPart(TextFormat.Html) { Text = "<html><head></head><body><h3>" + model.Message + "</h3></body>" };

                // send email
                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("mailclone1007@gmail.com", "vantam1007");
                await smtp.SendAsync(email);
                return true;
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("FAIL:" + e);
            }finally
            {
                smtp.Disconnect(true);
                smtp.Dispose();
            }
            return false;

        }
    }
}
