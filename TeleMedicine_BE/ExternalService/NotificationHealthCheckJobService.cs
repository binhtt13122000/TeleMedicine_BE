using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Models;
using TeleMedicine_BE.ExternalService;
using System.Collections.Generic;

namespace BusinessLogic.Services
{
    [DisallowConcurrentExecution]
    class NotificationHealthCheckJobService : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationHealthCheckJobService> _logger;

        public NotificationHealthCheckJobService(IServiceProvider serviceProvider, ILogger<NotificationHealthCheckJobService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        public Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                DateTime currentDate = DateTime.Today;
                TimeSpan addSevenHour = new TimeSpan(7, 0, 0);
                TimeSpan addOneHour = new TimeSpan(1, 0, 0);

                TimeSpan totalTime = DateTime.Now.TimeOfDay;
                TimeSpan currentTime = new TimeSpan(totalTime.Hours, totalTime.Minutes, totalTime.Seconds);

                _logger.LogInformation("Current Time:" + currentTime);
                if (totalTime.Days > 0 )
                {
                    currentDate = currentDate.AddDays(totalTime.Days);
                    _logger.LogInformation("Current Date:" + currentDate);
                }  
                    ISendEmailService sendEmailService = scope.ServiceProvider.GetService<ISendEmailService>();
                    IHealthCheckService healthCheckService = scope.ServiceProvider.GetService<IHealthCheckService>();
                    IPushNotificationService pushNotification = scope.ServiceProvider.GetService<IPushNotificationService>();
                    IDoctorService doctorService = scope.ServiceProvider.GetService<IDoctorService>();
                    ISlotService slotService = scope.ServiceProvider.GetService<ISlotService>();
                    var slots = slotService.GetAll(s => s.HealthCheck, s => s.Doctor, s => s.HealthCheck.Patient).AsEnumerable().Where(s => s.HealthCheckId != null)
                                                                                    .Where(s => s.HealthCheck.Status.Equals("BOOKED"))
                                                                                    .Where(s => s.AssignedDate.CompareTo(currentDate) == 0)
                                                                                    .Where(s => s.StartTime.CompareTo(currentTime) >= 0
                                                                                    && s.StartTime.CompareTo(currentTime.Add(addOneHour)) <= 0).ToList();
                    _logger.LogInformation("Rating:" + slots.Count());
                    if (slots != null && slots.Count() > 0)
                    {
                        for (int i = 0; i < slots.Count(); i++)
                        {
                            EmailForm mailPatient = new EmailForm();
                            mailPatient.ToEmail = slots[i].HealthCheck.Patient.Email;
                            mailPatient.Subject = "Thông báo có lịch hẹn với bác sĩ sắp tới.";
                            mailPatient.Message = "Bạn sắp có 1 lịch hẹn diễn ra.Thời gian diễn ra bắt đầu lúc: " + slots[i].StartTime;
                            sendEmailService.SendEmail(mailPatient).Wait();

                            EmailForm mailDoctor = new EmailForm();
                            mailDoctor.ToEmail = slots[i].Doctor.Email;
                            mailDoctor.Subject = "Thông báo có lịch hẹn với bệnh nhận sắp tới.";
                            mailDoctor.Message = "Bạn sắp có 1 lịch hẹn diễn ra.Thời gian diễn ra bắt đầu lúc: " + slots[i].StartTime;
                            sendEmailService.SendEmail(mailDoctor).Wait();
                            _logger.LogInformation("Rating:" + slots[i].Doctor.Email);
                            _logger.LogInformation("Rating:" + slots[i].HealthCheck.Patient.Email);
                            pushNotification.SendMessage("Bạn sắp có 1 lịch hẹn diễn ra.", "Thời gian diễn ra bắt đầu lúc: " + slots[i].StartTime, slots[i].Doctor.Email.ToLower(), null).Wait();
                            pushNotification.SendMessage("Bạn có cuộc hẹn với bác sĩ " + slots[i].Doctor.Name, "Thời gian diễn ra bắt đầu lúc: " + slots[i].StartTime, slots[i].HealthCheck.Patient.Email, null).Wait();
                        }
                    }
            }
            return Task.CompletedTask;
        }
    }
}
