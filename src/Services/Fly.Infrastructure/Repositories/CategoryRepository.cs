using Fly.Application.Common.Interfaces;
using Fly.Domain.Aggreagates;
using Fly.Domain.SeedWork;

namespace Fly.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public IUnitOfWork UnitOfWork => _context;
        private readonly IFlyMongoDbContext _context;

        public CategoryRepository(IFlyMongoDbContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
        }
    }
}
