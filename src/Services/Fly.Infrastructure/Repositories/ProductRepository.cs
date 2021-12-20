using Fly.Application.Common.Interfaces;
using Fly.Domain.Aggreagates;
using Fly.Domain.Entities;
using Fly.Domain.SeedWork;

namespace Fly.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        public IUnitOfWork UnitOfWork => _context;
        private readonly IFlyMongoDbContext _context;

        public ProductRepository(IFlyMongoDbContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
        }
    }
}
