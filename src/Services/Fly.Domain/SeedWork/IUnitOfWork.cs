using Fly.Domain.Abstracts;
using Fly.Domain.Common;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Fly.Domain.SeedWork
{
    public interface IUnitOfWork
    {
        Task<bool> SaveAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : EntityStringKey;

        Task<bool> SaveAsync<TEntity>(TEntity entity,
        RecordOption recordOption = RecordOption.Upsert, Expression<Func<TEntity, bool>>? filter = null,
            List<PlanetUpdate>? planetUpdateDefinitions = null, CancellationToken cancellationToken = default)
        where TEntity : EntityStringKey;

        Task<bool> SaveAllAsync<TEntity>(IEnumerable<TEntity> entities,
            RecordOption recordOption = RecordOption.Upsert, Expression<Func<TEntity, bool>>? filter = null,
            List<PlanetUpdate>? planetUpdateDefinitions = null, CancellationToken cancellationToken = default)
            where TEntity : EntityStringKey;

        Task<bool> SaveAsync<TEntity, TItem>(TEntity entity,
            RecordOption recordOption = RecordOption.Upsert, Expression<Func<TEntity, bool>>? filter = null,
            List<PlanetUpdate<TItem>>? planetUpdateDefinitions = null, CancellationToken cancellationToken = default)
            where TEntity : EntityStringKey where TItem : class;

        Task<bool> SaveAllAsync<TEntity, TItem>(IEnumerable<TEntity> entities,
            RecordOption recordOption = RecordOption.Upsert, Expression<Func<TEntity, bool>>? filter = null,
            List<PlanetUpdate<TItem>>? planetUpdateDefinitions = null, CancellationToken cancellationToken = default)
            where TEntity : EntityStringKey
            where TItem : class;

        Task<bool> SaveAllAsync<TEntity>(IEnumerable<TEntity> entities, RecordOption recordOption = RecordOption.Upsert,
            Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
            where TEntity : EntityStringKey;
        Task<bool> SaveAllAsync<TEntity>(IEnumerable<TEntity> entities, InsertManyOptions? insertManyOptions = null, CancellationToken cancellationToken = default)
            where TEntity : EntityStringKey;

        Task DeleteAsync<TEntity>(string id, CancellationToken cancellationToken = default) where TEntity : EntityStringKey;
        Task<TEntity> GetAsync<TEntity>(string id, CancellationToken cancellationToken = default) where TEntity : EntityStringKey;
        Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(AggregateOptions options = null, CancellationToken cancellationToken = default) where TEntity : EntityStringKey;
        Task<int> CountAsync<TEntity>(AggregateOptions? options = null,
            Expression<Func<TEntity, bool>>? prediction = null, CancellationToken cancellationToken = default)
            where TEntity : EntityStringKey;
        Task<bool> AnyAsync<TEntity>(AggregateOptions? options = null, Expression<Func<TEntity, bool>>? prediction = null,
            CancellationToken cancellationToken = default) where TEntity : EntityStringKey;
    }
}
