using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.Utils
{
    public interface IRedisService<T>
    {
        Task<T> Get(string key);
        Task<IDictionary<string, T>> GetList(string pattern);

        Task<bool> Set(string key, T value, double time);

        Task<bool> SetList(List<Tuple<string, T>> data, double time);

        Task<bool> RemoveKey(string key);

        Task<long> RemoveKeys(string pattern);
        Task<List<string>> GetAllKeys(string pattern);
        Task<int> Count(string pattern);
    }
    public class RedisService<T> : IRedisService<T> where T : class
    {
        private readonly IRedisCacheClient _redisCacheClient;

        public RedisService(IRedisCacheClient redisCacheClient)
        {
            _redisCacheClient = redisCacheClient;
        }

        public async Task<int> Count(string pattern)
        {
            List<string> keys = await GetAllKeys(pattern);

            return keys.Count;
        }

        public async Task<T> Get(string key)
        {
            T data = await _redisCacheClient.GetDbFromConfiguration().GetAsync<T>(key);

            return data;
        }

        public async Task<List<string>> GetAllKeys(string pattern)
        {
            List<string> allKeys = (await _redisCacheClient.GetDbFromConfiguration().SearchKeysAsync(pattern)).ToList();
            return allKeys;
        }

        public async Task<IDictionary<string, T>> GetList(string pattern)
        {
            List<string> keys = await GetAllKeys(pattern);

            IDictionary<string, T> dataFromCache = await _redisCacheClient.GetDbFromConfiguration().GetAllAsync<T>(keys);

            return dataFromCache;
        }

        public async Task<bool> RemoveKey(string key)
        {
            bool success = await _redisCacheClient.GetDbFromConfiguration().RemoveAsync(key);
            return success;
        }

        public async Task<long> RemoveKeys(string pattern)
        {
            List<string> keys = await GetAllKeys(pattern);
            long success = await _redisCacheClient.GetDbFromConfiguration().RemoveAllAsync(keys);
            return success;
        }

        public async Task<bool> Set(string key, T value, double time)
        {
            bool success = await _redisCacheClient.GetDbFromConfiguration().AddAsync<T>(key, value, DateTimeOffset.Now.AddMinutes(time));
            return success;
        }

        public async Task<bool> SetList(List<Tuple<string, T>> data, double time)
        {
            bool success = await _redisCacheClient.GetDbFromConfiguration().AddAllAsync(data, DateTimeOffset.Now.AddMinutes(time));
            return success;
        }
    }
}
