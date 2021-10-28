using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TeleMedicine_BE.Utils
{
    public class BackgroundWork : BackgroundService
    {
        private readonly IWorker worker;

        public BackgroundWork(IWorker worker)
        {
            this.worker = worker;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await worker.DoWork(stoppingToken);
        }
    }
}
