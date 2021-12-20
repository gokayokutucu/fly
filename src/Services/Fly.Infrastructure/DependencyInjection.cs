using Fly.Domain.Aggreagates;
using Fly.Domain.Services;
using Fly.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Fly.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();

            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<ICategoryService, CategoryService>();

            return services;
        }
    }
}
