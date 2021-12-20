using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Fly.Application.Common.Interfaces;
using Fly.Persistence.Context;
using Planet.MongoDbCore;

namespace Fly.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDatabaseContexts(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("ConnectionStrings")
                .GetSection("MongoDbConnection")
                .Get<MongoDbContextConfiguration>();

            services.AddSingleton<IFlyMongoDbContext>(s =>
                new FlyMongoDbContext(
                    config.DatabaseName,
                    //config.Url,
                    s.GetService<ILogger<FlyMongoDbContext>>()));
            services.AddSingleton<IFlyMongoDbContext>(s =>
                new FlyMongoDbContext(
                    config.DatabaseName,
                    //config.Url,
                    s.GetService<ILogger<FlyMongoDbContext>>()));

            return services;
        }
    }
}
