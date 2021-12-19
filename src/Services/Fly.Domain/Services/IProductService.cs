using Fly.Domain.Entities;

namespace Fly.Domain.Services
{
    public interface IProductService
    {
        Task<IList<Product>> GetAsync();
        Task CreateAsync(Product data);
    }
}