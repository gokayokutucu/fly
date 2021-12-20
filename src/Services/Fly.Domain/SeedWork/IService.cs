namespace Fly.Domain.SeedWork
{
    public interface IService<T>
    {
        Task<List<T>> GetAsync(CancellationToken cancellationToken = default);
        Task<T> GetAsync(string id, CancellationToken cancellationToken = default);
        Task CreateAsync(T data, CancellationToken cancellationToken = default);
        Task UpdateAsync(T data, CancellationToken cancellationToken = default);
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}
