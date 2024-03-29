﻿using FirebaseAdmin.Messaging;
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
            _redisService = redisService;
        }

        public async Task DoWork(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    IDictionary<string, Message> notifications = await _redisService.GetList<Message>("notification*");
                    foreach (var notification in notifications)
                    {
                        var messaging = FirebaseMessaging.DefaultInstance;
                        System.Diagnostics.Debug.WriteLine(notification.Value.ToString());
                        await messaging.SendAsync(notification.Value);
                    }
                    await _redisService.RemoveKeys("notification*");
                    await Task.Delay(1000 * 5);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("error");
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                }
                finally
                {
                }
            }
        }
    }
}
