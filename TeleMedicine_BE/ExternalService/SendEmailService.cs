
using Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

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
            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Host = "in-v3.mailjet.com";
            client.Port = 587;

            NetworkCredential credentials =
                new NetworkCredential("2b25e32971baaf5bc39f26627fe6c6a6", "749be964508d3f95a61e0c41eee48b68");
            client.UseDefaultCredentials = false;
            client.Credentials = credentials;

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("mailclone1007@gmail.com", "Tele-Medicine");
            msg.To.Add(new MailAddress(model.ToEmail));
            msg.Priority = MailPriority.High;

            msg.Subject = model.Subject;
            msg.IsBodyHtml = true;
            msg.Body = string.Format("<html><head></head><body><h3>" + model.Message + "</h3></body>");

            try
            {
                await client.SendMailAsync(msg);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }


            return false;

        }
    }
}