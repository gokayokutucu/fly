using Fly.Application.Common.Mappings;
using Fly.Application.DomainEvents.Products.Commands.AddProduct;
using Fly.Common.Behaviors;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Fly.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            #region Assembly Injections
            // Add AutoMapper
            services.AddAutoMapper(typeof(MapperProfile).GetTypeInfo().Assembly);

            // Add MediatR
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            services.AddMediatR(typeof(CreateProductCommand).GetTypeInfo().Assembly);
            #endregion

            return services;
        }
    }
}
