using Fly.Domain.Aggreagates;
using Fly.Domain.Entities;

namespace Fly.Domain.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepo;

        public CategoryService(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<List<Category>> GetAsync(CancellationToken cancellationToken = default)
        {
            var list = await _categoryRepo.UnitOfWork.GetAllAsync<Category>(cancellationToken: cancellationToken);

            return list.ToList();
        }

        public async Task<Category> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _categoryRepo.UnitOfWork.GetAsync<Category>(id, cancellationToken);
        }

        public async Task CreateAsync(Category data, CancellationToken cancellationToken = default)
        {
            await _categoryRepo.UnitOfWork.SaveAsync(data, Common.RecordOption.Insert, null, null, cancellationToken);
        }

        public async Task UpdateAsync(Category data, CancellationToken cancellationToken = default)
        {
            await _categoryRepo.UnitOfWork.SaveAsync(data, Common.RecordOption.Update, null, null, cancellationToken);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            await _categoryRepo.UnitOfWork.DeleteAsync<Category>(id, cancellationToken);
        }
    }
}
