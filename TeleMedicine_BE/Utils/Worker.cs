using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TeleMedicine_BE.Utils
{
    public interface IWorker
    {
        Task DoWork(CancellationToken cancellationToken);
    }
    public class Worker : IWorker
    {
        private int number = 0;

        public Worker()
        {
            number = 0;
        }

        public async Task DoWork(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                System.Diagnostics.Debug.WriteLine("cc");
                System.Diagnostics.Debug.WriteLine(number);
                number++;
                await Task.Delay(1000 * 5);
            }
        }
    }
}
