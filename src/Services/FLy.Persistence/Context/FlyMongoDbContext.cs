using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Fly.Application.Common.Interfaces;
using Fly.Domain.Entities;
using Fly.Persistence.BsonMaps;
using Fly.Persistence.Configurations;
using Planet.MongoDbCore;
using Planet.MongoDbCore.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Fly.Domain.Abstracts;
using Fly.Domain.Common;
using Fly.Common.Extensions;

namespace Fly.Persistence.Context
{
    public class FlyMongoDbContext : MongoDbContext, IFlyMongoDbContext
    {
        private static readonly object InitializationLock = new object();
        private readonly ILogger<FlyMongoDbContext> _logger;

        public FlyMongoDbContext(MongoDbContextOptions options) : base(options) { }

        public FlyMongoDbContext(MongoDbContextOptions options, ILogger<FlyMongoDbContext> logger) : base(options)
        {
            _logger = logger;
        }

        public FlyMongoDbContext(string databaseName, string url) : base(databaseName, url) { }

        public FlyMongoDbContext(string databaseName, string url, ILogger<FlyMongoDbContext> logger) : base(databaseName, url)
        {
            _logger = logger;
        }

        public FlyMongoDbContext(string databaseName, string host = "localhost", int port = 27017) : base(databaseName, host, port) { }

        public FlyMongoDbContext(string databaseName, ILogger<FlyMongoDbContext> logger, string host = "localhost", int port = 27017) : base(databaseName, host, port)
        {
            _logger = logger;
        }

        protected override async Task ConfigurateAsync()
        {
            lock (InitializationLock)
            {
                DatabaseBsonMap.Map();
            }

            try
            {
                await Task.Run(async () =>
                    await IndexConfigurations<FlyMongoDbContext>.CreateAllIndexes(this))
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Something is wrong in ConfigurateAsync");
            }
        }

        public IMongoCollection<Product> Products => GetCollection<Product>();
        public IMongoCollection<Category> Categories => GetCollection<Category>();

        public IMongoCollection<TEntity> GetCollection<TEntity>(CancellationToken cancellationToken = default) where TEntity : EntityStringKey
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return base.GetCollection<TEntity>();
            }
            catch (TimeoutException ex)
            {
                _logger?.LogError($"Timeout Exception in GetCollection method. Source: {ex.Source}");
                return default;
            }
            catch (MongoAuthenticationException ex)
            {
                _logger?.LogError($"Mongo Authentication Exception in GetCollection method. Source: {ex.Source}");
                return default;
            }
            catch (MongoConnectionException ex)
            {
                _logger?.LogError($"Mongo Connection Exception in GetCollection method. Source: {ex.Source}");
                return default;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Something is wrong in GetCollection");
                return default;
            }
        }

        public async Task<bool> SaveAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : EntityStringKey
        {
            try
            {
                var saveResult = await GetCollection<TEntity>().ReplaceOneAsync(r => r.Id.Equals(entity.Id), entity,
                    new ReplaceOptions() { IsUpsert = true }, cancellationToken);
                return saveResult.ModifiedCount > 0;
            }
            catch (TimeoutException ex)
            {
                _logger?.LogError($"Timeout Exception in SaveAsync method. Source: {ex.Source}");
                return false;
            }
            catch (MongoAuthenticationException ex)
            {
                _logger?.LogError($"Mongo Authentication Exception in SaveAsync method. Source: {ex.Source}");
                return false;
            }
            catch (MongoConnectionException ex)
            {
                _logger?.LogError($"Mongo Connection Exception in SaveAsync method. Source: {ex.Source}");
                return false;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex,
                    $"Entity Type: {typeof(TEntity)} | Entity ID:{entity.Id} - This record cannot be saved");
                return false;
            }
        }

        /// <summary>
        /// Insert or update entity on database
        /// </summary>
        /// <param name="entity">Entity model</param>
        /// <param name="recordOption">Insert, update or upsert operation</param>
        /// <param name="filter">Filter for update statement</param>
        /// <param name="planetUpdateDefinitions">Update fields and values</param>
        /// <param name="cancellationToken">Token</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns></returns>
        public async Task<bool> SaveAsync<TEntity>(TEntity entity,
            RecordOption recordOption = RecordOption.Upsert, Expression<Func<TEntity, bool>> filter = null,
            List<PlanetUpdate> planetUpdateDefinitions = null, CancellationToken cancellationToken = default)
            where TEntity : EntityStringKey
        {
            return await SaveAllAsync(new List<TEntity>() { entity }, recordOption, filter,
                planetUpdateDefinitions,
                cancellationToken);
        }

        /// <summary>
        /// Insert or update entity on database
        /// </summary>
        /// <param name="entity">Entity model</param>
        /// <param name="recordOption">Insert, update or upsert operation</param>
        /// <param name="filter">Filter for update statement</param>
        /// <param name="planetUpdateDefinitions">Update fields and values</param>
        /// <param name="cancellationToken">Token</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TItem">PlanetUpdate type</typeparam>
        /// <returns></returns>
        public async Task<bool> SaveAsync<TEntity, TItem>(TEntity entity,
            RecordOption recordOption = RecordOption.Upsert, Expression<Func<TEntity, bool>> filter = null,
            List<PlanetUpdate<TItem>> planetUpdateDefinitions = null, CancellationToken cancellationToken = default)
            where TEntity : EntityStringKey where TItem : class
        {
            return await SaveAllAsync(new List<TEntity>() { entity }, recordOption, filter,
                planetUpdateDefinitions,
                 cancellationToken);
        }

        /// <summary>
        /// Insert or update all entities on database
        /// </summary>
        /// <param name="entities">Entity list model</param>
        /// <param name="recordOption">Insert, update or upsert operation</param>
        /// <param name="filter">Filter for update statement</param>
        /// <param name="planetUpdateDefinitions">Update fields and values</param>
        /// <param name="cancellationToken">Token</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns></returns>
        public async Task<bool> SaveAllAsync<TEntity>(IEnumerable<TEntity> entities, RecordOption recordOption = RecordOption.Upsert, Expression<Func<TEntity, bool>> filter = null, List<PlanetUpdate> planetUpdateDefinitions = null, CancellationToken cancellationToken = default) where TEntity : EntityStringKey
        {
            try
            {
                var collection = GetCollection<TEntity>();

                switch (recordOption)
                {
                    case RecordOption.Insert:
                        await collection.InsertManyAsync(entities, new InsertManyOptions() { IsOrdered = true }, cancellationToken);
                        return true;
                    case RecordOption.Update:
                        {
                            var filterBuilder = filter == null ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Where(filter);
                            var update = Builders<TEntity>.Update;
                            var updateDefinitions = planetUpdateDefinitions?
                                .Select(e =>
                                        new {e, fieldValue = e.FieldValue.CastToReflected(e.DataType)})
                                .Select(@t => update.Set(@t.e.FieldName, @t.fieldValue))
                                .Cast<UpdateDefinition<TEntity>>().ToList();

                            var updateResult = await collection.UpdateManyAsync(filterBuilder, update.Combine(updateDefinitions), cancellationToken: cancellationToken);
                            return updateResult.ModifiedCount > 0;
                        }
                }

                var bulkOps = entities
                    .Select(entity =>
                        new ReplaceOneModel<TEntity>(Builders<TEntity>.Filter.Where(x => x.Id == entity.Id), entity) { IsUpsert = true })
                    .Cast<WriteModel<TEntity>>()
                    .ToList();
                var bulkResult = await collection.BulkWriteAsync(bulkOps, cancellationToken: cancellationToken);
                return bulkResult.ModifiedCount > 0;
            }
            catch (TimeoutException ex)
            {
                _logger?.LogError($"Timeout Exception in SaveAllAsync method. Source: {ex.Source}");
                return false;
            }
            catch (MongoAuthenticationException ex)
            {
                _logger?.LogError($"Mongo Authentication Exception in SaveAllAsync method. Source: {ex.Source}");
                return false;
            }
            catch (MongoConnectionException ex)
            {
                _logger?.LogError($"Mongo Connection Exception in SaveAllAsync method. Source: {ex.Source}");
                return false;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"These records cannot be saved");
                return false;
            }
        }

        /// <summary>
        /// Insert or update all entities on database
        /// </summary>
        /// <param name="entities">Entity list model</param>
        /// <param name="recordOption">Insert, update or upsert operation</param>
        /// <param name="filter">Filter for update statement</param>
        /// <param name="planetUpdateDefinitions">Update fields and values</param>
        /// <param name="cancellationToken">Token</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TItem">PlanetUpdate type</typeparam>
        /// <returns></returns>
        public async Task<bool> SaveAllAsync<TEntity, TItem>(IEnumerable<TEntity> entities, RecordOption recordOption = RecordOption.Upsert, Expression<Func<TEntity, bool>> filter = null, List<PlanetUpdate<TItem>> planetUpdateDefinitions = null, CancellationToken cancellationToken = default) where TEntity : EntityStringKey where TItem : class
        {
            try
            {
                var collection = GetCollection<TEntity>();

                switch (recordOption)
                {
                    case RecordOption.Insert:
                        await collection.InsertManyAsync(entities, new InsertManyOptions() { IsOrdered = true }, cancellationToken);
                        return true;
                    case RecordOption.Update:
                        {
                            var filterBuilder = filter == null ? Builders<TEntity>.Filter.Empty : Builders<TEntity>.Filter.Where(filter);
                            var update = Builders<TEntity>.Update;
                            var updateDefinitions = planetUpdateDefinitions?.Select(e => update.Set(e.FieldName, e.FieldValue));

                            var updateResult = await collection.UpdateManyAsync(filterBuilder, update.Combine(updateDefinitions), cancellationToken: cancellationToken);
                            return updateResult.ModifiedCount > 0;
                        }
                }

                var bulkOps = entities
                    .Select(entity =>
                        new ReplaceOneModel<TEntity>(Builders<TEntity>.Filter.Where(x => x.Id == entity.Id), entity) { IsUpsert = true })
                    .Cast<WriteModel<TEntity>>()
                    .ToList();
                var bulkResult = await collection.BulkWriteAsync(bulkOps, cancellationToken: cancellationToken);
                return bulkResult.ModifiedCount > 0;
            }
            catch (TimeoutException ex)
            {
                _logger?.LogError($"Timeout Exception in SaveAllAsync method. Source: {ex.Source}");
                return false;
            }
            catch (MongoAuthenticationException ex)
            {
                _logger?.LogError($"Mongo Authentication Exception in SaveAllAsync method. Source: {ex.Source}");
                return false;
            }
            catch (MongoConnectionException ex)
            {
                _logger?.LogError($"Mongo Connection Exception in SaveAllAsync method. Source: {ex.Source}");
                return false;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"These records cannot be saved");
                return false;
            }
        }

        /// <summary>
        /// Insert or update all entities on database
        /// </summary>
        /// <param name="entities">Entity list model</param>
        /// <param name="recordOption">Insert, update or upsert operation</param>
        /// <param name="filter">Filter for update statement</param>
        /// <param name="cancellationToken">Token</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns></returns>
        public async Task<bool> SaveAllAsync<TEntity>(IEnumerable<TEntity> entities, RecordOption recordOption = RecordOption.Upsert, Expression<Func<TEntity, bool>> filter = null, CancellationToken cancellationToken = default) where TEntity : EntityStringKey
        {
            try
            {
                var collection = GetCollection<TEntity>();

                if (recordOption == RecordOption.Insert)
                {
                    await collection.InsertManyAsync(entities, new InsertManyOptions() { IsOrdered = true },
                        cancellationToken);
                    return true;
                }

                var bulkOps = entities
                    .Select(entity =>
                        new ReplaceOneModel<TEntity>(Builders<TEntity>.Filter.Where(x => x.Id == entity.Id), entity)
                        { IsUpsert = true })
                    .Cast<WriteModel<TEntity>>()
                    .ToList();
                var bulkResult = await collection.BulkWriteAsync(bulkOps, cancellationToken: cancellationToken);
                return bulkResult.ModifiedCount > 0;
            }
            catch (TimeoutException ex)
            {
                _logger?.LogError($"Timeout Exception in SaveAllAsync method. Source: {ex.Source}");
                return false;
            }
            catch (MongoAuthenticationException ex)
            {
                _logger?.LogError($"Mongo Authentication Exception in SaveAllAsync method. Source: {ex.Source}");
                return false;
            }
            catch (MongoConnectionException ex)
            {
                _logger?.LogError($"Mongo Connection Exception in SaveAllAsync method. Source: {ex.Source}");
                return false;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"These records cannot be saved");
                return false;
            }
        }

        /// <summary>
        /// Insert all entities to database
        /// </summary>
        /// <param name="entities">Entity list model</param>
        /// <param name="insertManyOptions">To set insert options</param>
        /// <param name="cancellationToken">Token</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns></returns>
        public async Task<bool> SaveAllAsync<TEntity>(IEnumerable<TEntity> entities, InsertManyOptions insertManyOptions = default, CancellationToken cancellationToken = default) where TEntity : EntityStringKey
        {
            try
            {
                await GetCollection<TEntity>().InsertManyAsync(entities, insertManyOptions, cancellationToken);
                return true;
            }
            catch (TimeoutException ex)
            {
                _logger?.LogError($"Timeout Exception in SaveAllAsync method. Source: {ex.Source}");
                return false;
            }
            catch (MongoAuthenticationException ex)
            {
                _logger?.LogError($"Mongo Authentication Exception in SaveAllAsync method. Source: {ex.Source}");
                return false;
            }
            catch (MongoConnectionException ex)
            {
                _logger?.LogError($"Mongo Connection Exception in SaveAllAsync method. Source: {ex.Source}");
                return false;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"These records cannot be saved");
                return false;
            }
        }

        public new IMongoQueryable<TEntity> AllQueryable<TEntity>(AggregateOptions? options = null, CancellationToken cancellationToken = default) where TEntity : EntityStringKey
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                return base.AllQueryable<TEntity>();
            }
            catch (TimeoutException ex)
            {
                _logger?.LogError($"Timeout Exception in AllQueryable method. Source: {ex.Source}");
                return default;
            }
            catch (MongoAuthenticationException ex)
            {
                _logger?.LogError($"Mongo Authentication Exception in AllQueryable method. Source: {ex.Source}");
                return default;
            }
            catch (MongoConnectionException ex)
            {
                _logger?.LogError($"Mongo Connection Exception in AllQueryable method. Source: {ex.Source}");
                return default;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Something is wrong in AllQueryable");
                return new EmptyMongoQueryable<TEntity>();
            }
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(AggregateOptions? options = null, CancellationToken cancellationToken = default) where TEntity : EntityStringKey
        {
            var res = await AllQueryable<TEntity>(options, cancellationToken).ToListAsync<TEntity>(cancellationToken: cancellationToken);
            return  res;
        }

        public async Task<IEnumerable<TEntity>> GetByPagingAsync<TEntity>(IMongoQueryable<TEntity> queryable = null, int pageNumber = 1, int pageSize = 10, AggregateOptions options = null, CancellationToken cancellationToken = default) where TEntity : EntityStringKey
        {
            return await PaginatedList<TEntity>.CreateAsync(queryable ?? AllQueryable<TEntity>(options, cancellationToken),
                pageNumber, pageSize);
        }

        public async Task<TEntity> GetAsync<TEntity>(string id, CancellationToken cancellationToken = default) where TEntity : EntityStringKey
        {
            var collection = GetCollection<TEntity>();
            var filter = Builders<TEntity>.Filter.Eq("Id", id);
            return await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> GetAnyInAsync<TEntity, TField>(Expression<Func<TEntity, IEnumerable<TField>>> fieldExpression, IEnumerable<TField> values, CancellationToken cancellationToken = default) where TEntity : EntityStringKey
        {
            var collection = GetCollection<TEntity>();
            var filter = Builders<TEntity>.Filter.AnyIn(fieldExpression, values);

            var propertyValues = await collection
               .Find(filter)
               .ToListAsync(cancellationToken: cancellationToken);

            return propertyValues;
        }

        public async Task DeleteAsync<TEntity>(string id, CancellationToken cancellationToken = default) where TEntity : EntityStringKey
        {
            var filter = Builders<TEntity>.Filter.Eq("Id", id);
            await GetCollection<TEntity>().DeleteOneAsync(filter, cancellationToken);
        }

        public async Task<bool> UpdateAnArrayAsync<TEntity, TItem>(string id, Expression<Func<TEntity, IEnumerable<TItem>>> item, TItem value, CancellationToken cancellationToken = default)
        where TEntity : EntityStringKey
        where TItem : class
        {
            var update = Builders<TEntity>.Update.Push(item, value);
            var filter = Builders<TEntity>.Filter.Eq("Id", id);
            var result = await GetCollection<TEntity>().FindOneAndUpdateAsync(filter, update, null, cancellationToken);
            return result != null;
        }

        public async Task<int> CountAsync<TEntity>(AggregateOptions options = null, Expression<Func<TEntity, bool>> prediction = null, CancellationToken cancellationToken = default) where TEntity : EntityStringKey
        {
            if (prediction == null)
                return await AllQueryable<TEntity>(options, cancellationToken).CountAsync(cancellationToken);
            return await AllQueryable<TEntity>(options, cancellationToken).CountAsync(prediction, cancellationToken);
        }

        public async Task<bool> AnyAsync<TEntity>(AggregateOptions options = null, Expression<Func<TEntity, bool>> prediction = null, CancellationToken cancellationToken = default) where TEntity : EntityStringKey
        {
            if (prediction == null)
            {
                return await AllQueryable<TEntity>(options, cancellationToken).AnyAsync(cancellationToken);
            }
            return await AllQueryable<TEntity>(options, cancellationToken).AnyAsync(prediction, cancellationToken);
        }

        public async Task<bool> DocumentHasFieldAsync(string collectionName, string fieldName, CancellationToken cancellationToken = default)
        {
            var collection = base._database.GetCollection<BsonDocument>(collectionName);
            var document = await collection.Find(x => true).FirstOrDefaultAsync(cancellationToken);
            return document.ToBsonDocument().TryGetElement(fieldName, out _);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task CommitEntitiesAsync<TEntity>(List<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : EntityStringKey
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
