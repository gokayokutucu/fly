using Fly.Domain.Abstracts;
using Fly.Domain.Entities;
using Fly.Domain.SeedWork;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace Fly.Application.Common.Interfaces
{
    public interface IFlyMongoDbContext : IUnitOfWork
    {
        IMongoCollection<Product> Products { get; }
        IMongoCollection<Category> Categories { get; }
        IMongoCollection<TEntity> GetCollection<TEntity>(CancellationToken cancellationToken = default) where TEntity : EntityStringKey;
        IMongoQueryable<TEntity> AllQueryable<TEntity>(AggregateOptions options = null, CancellationToken cancellationToken = default) where TEntity : EntityStringKey;
        
        Task<IEnumerable<TEntity>> GetAnyInAsync<TEntity, TField>(Expression<Func<TEntity, IEnumerable<TField>>> fieldExpression, IEnumerable<TField> values, CancellationToken cancellationToken = default) where TEntity : EntityStringKey;
        Task<IEnumerable<TEntity>> GetByPagingAsync<TEntity>(IMongoQueryable<TEntity> queryable = null, int pageNumber = 1, int pageSize = 10, AggregateOptions options = null, CancellationToken cancellationToken = default) where TEntity : EntityStringKey;
    }
}
