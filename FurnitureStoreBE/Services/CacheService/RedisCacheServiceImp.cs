using FurnitureStoreBE.Exceptions;
using StackExchange.Redis;
using System.Text.Json;
using System.Threading.Tasks;

namespace FurnitureStoreBE.Services.Caching
{
    public class RedisCacheServiceImp : IRedisCacheService
    {
        private readonly IDatabase _cache;
        private readonly IServer _server;

        public RedisCacheServiceImp(IConnectionMultiplexer redis)
        {
            _cache = redis.GetDatabase();
            _server = redis.GetServer(redis.GetEndPoints()[0]);
        }

        public async Task<T?> GetData<T>(string key)
        {

            var data = await _cache.StringGetAsync(key);
            if (data.IsNullOrEmpty)
            {
                return default;
            }

            try
            {
                var result = JsonSerializer.Deserialize<T>(data);
                return result;
            }
            catch (JsonException ex)
            {
                return default;
            }
        }

        public async Task SetData<T>(string key, T data, TimeSpan? expiry = null)
        {

            var serializedData = JsonSerializer.Serialize(data);
            var wasSet = await _cache.StringSetAsync(key, serializedData, expiry);

            if (wasSet)
                Console.WriteLine(serializedData);
        }

        public async Task RemoveData(string key)
        {
            var wasRemoved = await _cache.KeyDeleteAsync(key);
        }

        public async Task RemoveAllData()
        {
            await _server.FlushDatabaseAsync();
        }
    }
}
