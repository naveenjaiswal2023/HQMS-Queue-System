namespace HQMS.API.Domain.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null) where T : class;
        Task RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);

        // Generic cache update
        Task UpdateCacheAsync<T>(string key, T data, TimeSpan? expiry = null) where T : class;
        Task UpdateCacheAsync<T>(string keyPrefix, Guid id, T data, TimeSpan? expiry = null) where T : class;
    }
}
