using Fly.Domain.Entities;
using Fly.Domain.SeedWork;

namespace Fly.Domain.Aggreagates
{
    public interface IProductService : IService<Product>
    {
        Task<int> CountAsync(CancellationToken cancellationToken = default);
    }
}
