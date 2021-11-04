using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Infrastructure.Models;

namespace BusinessLogic.Services
{
    [DisallowConcurrentExecution]
    public class ReviewJobService : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReviewJobService> _logger;

        public ReviewJobService(IServiceProvider serviceProvider, ILogger<ReviewJobService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        public Task Execute(IJobExecutionContext context)
        {
            using(var scope = _serviceProvider.CreateScope())
            {
                IHealthCheckService healthCheckService = scope.ServiceProvider.GetService<IHealthCheckService>();
                IDoctorService doctorService = scope.ServiceProvider.GetService<IDoctorService>();
                IQueryable<Doctor> doctorList = doctorService.GetAll();
                doctorList.ToList().ForEach(item =>
                {
                    IQueryable<HealthCheck> healthChecks = healthCheckService.GetAll().Where(s => s.Slots.Any(s => s.DoctorId == item.Id) 
                                                                                                && s.Status.Equals("COMPLETED")
                                                                                                && s.Rating != null);
                    if(healthChecks != null && healthChecks.Count() > 0)
                    {
                        double rating = (healthChecks.Sum(s => s.Rating) * 1.0 / healthChecks.Count()) ?? 0;
                        Double ratingFixed = Math.Round((Double)rating, 1);
                        item.Rating = ratingFixed;
                        _logger.LogInformation("Rating:" + ratingFixed);
                        doctorService.UpdateAsync(item).Wait();
                    }
                });
            }
            return Task.CompletedTask;
        }
    }
}
