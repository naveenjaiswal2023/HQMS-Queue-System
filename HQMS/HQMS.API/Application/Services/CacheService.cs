using HQMS.API.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace HQMS.API.Application.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;

        }

        public async Task<T> GetAsync<T>(string key) where T : class
        {
            var cached = await _cache.GetStringAsync(key);
            if (cached == null)
                return null;

            return JsonSerializer.Deserialize<T>(cached);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null) where T : class
        {
            var json = JsonSerializer.Serialize(value);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromMinutes(30),
                SlidingExpiration = slidingExpiration
            };

            await _cache.SetStringAsync(key, json, options);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public async Task UpdateCacheAsync<T>(string key, T data, TimeSpan? expiry = null) where T : class
        {
            await SetAsync(key, data, expiry);
        }

        public async Task UpdateCacheAsync<T>(string keyPrefix, Guid id, T data, TimeSpan? expiry = null) where T : class
        {
            var key = $"{keyPrefix}:{id}";
            await SetAsync(key, data, expiry);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            var cachedValue = await _cache.GetStringAsync(key);
            return !string.IsNullOrEmpty(cachedValue);
        }
    }
}
