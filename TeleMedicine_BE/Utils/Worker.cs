using FirebaseAdmin.Messaging;
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

        private readonly IRedisService _redisService;

        public Worker(IRedisService redisService)
        {
            number = 0;
            _redisService = redisService;
        }

        public async Task DoWork(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                System.Diagnostics.Debug.WriteLine("cc");
                System.Diagnostics.Debug.WriteLine(number);
                number++;
                IDictionary<string, Message> notifications = await _redisService.GetList<Message>("notification*");
                foreach(var notification in notifications)
                {
                    var messaging = FirebaseMessaging.DefaultInstance;
                    await messaging.SendAsync(notification.Value);
                }
                await _redisService.RemoveKeys("notification*");
                await Task.Delay(1000 * 30);
            }
        }
    }
}
