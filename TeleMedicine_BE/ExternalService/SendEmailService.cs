using Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
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
            try
            {
                var apiKey = _configuration["SendGridAPILey"];
                var client = new SendGridClient(apiKey);
                System.Diagnostics.Debug.WriteLine("SomeText: " + apiKey);
                var from = new EmailAddress("danhskipper18@gmail.com", "Tele-Medicine");
                var subject = model.Subject;
                var to = new EmailAddress(model.ToEmail);
                var htmlContent = "<html><head></head><body><h3>" + model.Message + "</h3></body>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, model.Message, htmlContent);
                var response = await client.SendEmailAsync(msg);
                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine("SUCCESS:");
                    return true;
                }
            }catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("FAIL:" + e);
            }
            return false;

        }
    }
}
