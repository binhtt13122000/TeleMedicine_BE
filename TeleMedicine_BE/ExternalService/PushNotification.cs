using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeleMedicine_BE.Utils;

namespace TeleMedicine_BE.ExternalService
{
    public interface IPushNotificationService
    {
        Task<bool> SendMessage(string title, string body, string email, Dictionary<String, String> additionalDatas);
    }
    public class PushNotificationService : IPushNotificationService
    {
        private readonly IRedisService _redisService;
        public PushNotificationService(IRedisService redisService)
        {
            _redisService = redisService;
        }
        public async Task<bool> SendMessage(string title, string body, string email, Dictionary<String, String> additionalDatas)
        {
            string token = await _redisService.Get<string>("user:" + email);
            if (token != null)
            {
                var message = new Message()
                {
                    Notification = new Notification()
                    {
                        Title = title,
                        Body = body,
                        ImageUrl = "https://png.pngtree.com/element_our/20190530/ourlarge/pngtree-520-couple-avatar-boy-avatar-little-dinosaur-cartoon-cute-image_1263411.jpg",
                    },
                    Token = token,
                    Data = additionalDatas,
                };
                string format = "Mddyyyyhhmmsstt";
                string key = string.Format("{0}", DateTime.Now.ToString(format));
                System.Diagnostics.Debug.WriteLine(key);
                return await _redisService.Set<Message>("notification:" + key, message, 60);
            }
            return false;
        }



    }
}