using Microsoft.Extensions.Caching.Memory;

namespace MyGymProject.Client.Services
{
    public class CacheService
    {
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;

        public CacheService(IMemoryCache cache, HttpClient httpClient)
        {
            _cache = cache;
            _httpClient = httpClient;
        }

        public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan expiry)
        {
            if (!_cache.TryGetValue(key, out T? cachedData))
            {
                cachedData = await factory();
                if (cachedData != null)
                {
                    _cache.Set(key, cachedData, expiry);
                }
            }
            return cachedData;
        }
    }
}
