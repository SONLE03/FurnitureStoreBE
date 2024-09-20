namespace FurnitureStoreBE.Services.Caching
{
    public interface IRedisCacheService
    {
        Task SetData<T>(string key, T data, TimeSpan? expiry = null);
        Task<T?> GetData<T>(string key);
        Task RemoveData(string key);
        Task RemoveAllData();
    }
}
