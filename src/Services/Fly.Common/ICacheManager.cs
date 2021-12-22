using StackExchange.Redis;

namespace Fly.Common
{
    public interface ICacheServerManager
    {
        ConnectionMultiplexer Connection { get; }
        Task ForceReconnectAsync();
        Task<IDatabase> GetDatabaseAsync();
        Task<System.Net.EndPoint[]> GetEndPointsAsync();
        Task<IServer> GetServerAsync(string host, int port);
    }

    public interface ICacheManager 
    {
        Task<T> GetAsync<T>(string key);
        Task<bool> DeleteAsync(string key);
        Task<bool> SetAsync<T>(string key, T data);
        Task<bool> SetExpireAsync<T>(string key, T data, TimeSpan expire);
    }
}