using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Fly.Domain.Enums;
using Fly.Domain.Exceptions;
using Planet.MongoDbCore;
using System.Linq.Expressions;
using Fly.Domain.Abstracts;

namespace Fly.Persistence.Configurations
{
    public class IndexConfigurations<TContext>
    where TContext : MongoDbContext
    {
        public static async Task CreateAllIndexes(TContext context, ILogger<TContext> logger = default, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                var dbContextType = typeof(TContext);

                // (IQueryable)Activator.CreateInstance(typeof(Queryable<>).
                //   MakeGenericType(elementType), new object[] { this, expression });

                var propertyInfos0 = dbContextType
                    .GetProperties();
                try
                {
                    var methodInfo = typeof(IndexConfigurations<TContext>).GetMethod("EntityIndexAsync");

                    foreach (var property in propertyInfos0)
                    {
                        var genericType = property.PropertyType.GenericTypeArguments.FirstOrDefault();
                        var mi = dbContextType.GetMethod("GetCollection", Type.EmptyTypes);
                        var miConstructed = mi?.MakeGenericMethod(genericType);
                        var collection = miConstructed?.Invoke(context, null);

                        var methodInfoConstructed = methodInfo?.MakeGenericMethod(genericType);
                        methodInfoConstructed?.Invoke(null, new[] { collection, logger, cancellationToken });
                    }
                }
                catch (Exception ex)
                {
                    throw new FlyDomainException("IndexConfigurations",
                        "Runtime error. Some types cannot be resolved", ex);
                }
            }, cancellationToken);
        }

        //IN USE! Calling this method in CreateAllIndexes by reflection GetMethod
        public static async Task EntityIndexAsync<TEntity>(IMongoCollection<TEntity> collection, ILogger<TContext> logger = default, CancellationToken cancellationToken = default) where TEntity : EntityStringKey
        {
            await CreateIndexAsync(collection, (e => e.LastModifiedDate), null, IndexKeyType.Ascending, logger, cancellationToken).ConfigureAwait(false);
        }

        //https://stackoverflow.com/questions/35019313/checking-if-an-index-exists-in-mongodb
        public static async Task CreateIndexAsync<TEntity>(IMongoCollection<TEntity> collection, Expression<Func<TEntity, object>> field,
            CreateOneIndexOptions createOneIndexOptions = null, IndexKeyType indexKeyType = IndexKeyType.Ascending, ILogger<TContext> logger = default, CancellationToken cancellationToken = default) where TEntity : EntityStringKey
        {
            if (field == null)
                throw new ArgumentNullException($"Field expression cannot be empty");

            CreateIndexModel<TEntity> indexModel;
            switch (indexKeyType)
            {
                case IndexKeyType.Descending:
                    indexModel = new CreateIndexModel<TEntity>(Builders<TEntity>.IndexKeys.Descending(field));
                    break;
                case IndexKeyType.Text:
                    indexModel = new CreateIndexModel<TEntity>(Builders<TEntity>.IndexKeys.Text(field));
                    break;
                case IndexKeyType.Hashed:
                    indexModel = new CreateIndexModel<TEntity>(Builders<TEntity>.IndexKeys.Hashed(field));
                    break;
                case IndexKeyType.Wildcard:
                    indexModel = new CreateIndexModel<TEntity>(Builders<TEntity>.IndexKeys.Wildcard(field));
                    break;
                default:
                    indexModel = new CreateIndexModel<TEntity>(Builders<TEntity>.IndexKeys.Ascending(field));
                    break;
            }
            try
            {
                await collection.Indexes
                    .CreateOneAsync(indexModel, createOneIndexOptions, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (MongoConnectionException mongoConnectionException)
            {
                logger?.LogError(mongoConnectionException, "Fly MongoDb database connection error");
            }
            catch (TimeoutException timeoutException)
            {
                logger?.LogError(timeoutException, "Timeout Exception in CreateIndexAsync method");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Something is wrong is MongoDB");
            }
        }
    }
}