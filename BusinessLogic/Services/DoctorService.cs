using Infrastructure.Models;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public interface IDoctorService: IService<Doctor, int>
    {
        Doctor GetDoctorByEmail(string email);
        DbSet<Doctor> access();

        public Task<bool> SendEmail(EmailForm model);
    }
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorService(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task<bool> SendEmail(EmailForm model)
        {
            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Host = "smtp.gmail.com";
            client.Port = 587;

            NetworkCredential credentials =
                new NetworkCredential("mailclone1007@gmail.com", "vantam1007");
            client.UseDefaultCredentials = false;
            client.Credentials = credentials;

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("mailclone1007@gmail.com", "Tele-Medicine");
            msg.To.Add(new MailAddress(model.ToEmail));

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

        public DbSet<Doctor> access()
        {
            return _doctorRepository.access();
        }

        public async Task<Doctor> AddAsync(Doctor entity)
        {
            return await _doctorRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(Doctor entity)
        {
            return await _doctorRepository.Delete(entity);
        }

        public IQueryable<Doctor> GetAll(params Expression<Func<Doctor, object>>[] includes)
        {
            return _doctorRepository.GetAll(includes);
        }

        public async Task<Doctor> GetByIdAsync(int id)
        {
            return await _doctorRepository.GetByIdAsync(id);
        }

        public Doctor GetDoctorByEmail(string email)
        {
            return _doctorRepository.GetAll().Where(x => x.Email.ToUpper().Equals(email.ToUpper())).FirstOrDefault();
        }

        public async Task<bool> UpdateAsync(Doctor entity)
        {
            return await _doctorRepository.UpdateAsync(entity);
        }
    }
}
