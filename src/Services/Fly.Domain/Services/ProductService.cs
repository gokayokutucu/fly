using Fly.Domain.Aggreagates;
using Fly.Domain.Entities;

namespace Fly.Domain.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;

        public ProductService(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<List<Product>> GetAsync(CancellationToken cancellationToken = default)
        {
            var list = await _productRepo.UnitOfWork.GetAllAsync<Product>(cancellationToken:cancellationToken);

            return list.ToList();
        }

        public async Task<Product> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _productRepo.UnitOfWork.GetAsync<Product>(id, cancellationToken);
        }

        public async Task CreateAsync(Product data, CancellationToken cancellationToken = default)
        {
            await _productRepo.UnitOfWork.SaveAsync(data, Common.RecordOption.Insert, null, null, cancellationToken);
        }

        public async Task UpdateAsync(Product data, CancellationToken cancellationToken = default)
        {
            await _productRepo.UnitOfWork.SaveAsync(data, Common.RecordOption.Upsert, null, null, cancellationToken);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            await _productRepo.UnitOfWork.DeleteAsync<Product>(id, cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
           return await _productRepo.UnitOfWork.CountAsync<Product>(null, null, cancellationToken);
        }
    }
}