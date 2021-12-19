using Fly.Domain.Entities;

namespace Fly.Domain.Services
{
    public class ProductService : IProductService
    {
        public async Task<IList<Product>> GetAsync()
        {
            var products = new List<Product>();
            return await Task.FromResult(products);
        }

        public async Task CreateAsync(Product data)
        {
            throw new NotImplementedException();
        }
    }
}