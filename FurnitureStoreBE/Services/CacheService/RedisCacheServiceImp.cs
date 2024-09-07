using FurnitureStoreBE.Exceptions;
using StackExchange.Redis;
using System.Text.Json;
using System.Threading.Tasks;

namespace FurnitureStoreBE.Services.Caching
{
    public class RedisCacheServiceImp : IRedisCacheService
    {
        private readonly IDatabase _cache;

        public RedisCacheServiceImp(string connectionString)
        {
            var redis = ConnectionMultiplexer.Connect(connectionString);
            _cache = redis.GetDatabase();
        }

        public async Task<T?> GetData<T>(string key)
        {
            var data = await _cache.StringGetAsync(key);
            if (data.IsNullOrEmpty)
                return default;

            return JsonSerializer.Deserialize<T>(data);
        }

        public async Task SetData<T>(string key, T data, TimeSpan? expiry = null)
        {
            var serializedData = JsonSerializer.Serialize(data);
            await _cache.StringSetAsync(key, serializedData, expiry);
        }

        public async Task RemoveData(string key)
        {
            await _cache.KeyDeleteAsync(key);
        }
    }
}
