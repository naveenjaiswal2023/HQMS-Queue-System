using HQMS.API.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace HQMS.API.Application.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<T> GetAsync<T>(string key) where T : class
        {
            _cache.TryGetValue(key, out T value);
            return Task.FromResult(value);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null) where T : class
        {
            var options = new MemoryCacheEntryOptions();

            if (absoluteExpiration.HasValue)
                options.SetAbsoluteExpiration(absoluteExpiration.Value);

            if (slidingExpiration.HasValue)
                options.SetSlidingExpiration(slidingExpiration.Value);

            _cache.Set(key, value, options);

            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }

        public Task UpdateCacheAsync<T>(string key, T data, TimeSpan? expiry = null) where T : class
        {
            return SetAsync(key, data, expiry);
        }

        public Task UpdateCacheAsync<T>(string keyPrefix, Guid id, T data, TimeSpan? expiry = null) where T : class
        {
            var key = $"{keyPrefix}:{id}";
            return SetAsync(key, data, expiry);
        }

        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(_cache.TryGetValue(key, out _));
        }
    }
}
