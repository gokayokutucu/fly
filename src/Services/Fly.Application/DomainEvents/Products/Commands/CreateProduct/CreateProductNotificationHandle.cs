using AutoMapper;
using Fly.Application.DomainEvents.Products.Dtos;
using Fly.Application.DomainEvents.Products.Queries.GetProducts;
using Fly.Application.Exceptions;
using Fly.Common;
using Fly.Common.Extensions;
using Fly.Domain.Aggreagates;
using Fly.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Fly.Application.DomainEvents.Products.Commands.AddProduct
{
    public class CreateProductDatabaseNotificationHandler : INotificationHandler<CreateProductNotification>
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public CreateProductDatabaseNotificationHandler(
            ILogger<CreateProductDatabaseNotificationHandler> logger,
            IMapper mapper,
            IProductService productService)
        {
            _logger = logger;
            _mapper = mapper;
            _productService = productService;
        }

        public async Task Handle(CreateProductNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var entity = _mapper.Map<Product>(notification.ProductDto);

                await _productService.CreateAsync(entity, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new FlyException(ex.Message, ex);
            }
        }
    }
    
    public class CreateProductMemoryCacheNotificationHandler : INotificationHandler<CreateProductNotification>
    {
        private readonly ILogger _logger;
        private readonly ICacheManager _cacheManager;
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;

        public CreateProductMemoryCacheNotificationHandler(
            ILogger<CreateProductMemoryCacheNotificationHandler> logger,
            ICacheManager cacheManager,
            IMapper mapper,
            ICategoryService categoryService)
        {
            _logger = logger;
            _cacheManager = cacheManager;
            _mapper = mapper;
            _categoryService = categoryService;
        }

        public async Task Handle(CreateProductNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    return;
                
                var keyName = $"product_{notification.ProductDto.Id}";

                var aggregate = await SetAggregateAsync(notification.ProductDto);

                if (aggregate is not null)
                    await _cacheManager.SetExpireAsync(keyName, aggregate, TimeSpan.FromMinutes(5));

                await UpdateCachedProductList(aggregate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new FlyException(ex.Message, ex);;
            }

        }

        private async Task<CreateProductDto> SetAggregateAsync(CreateProductDto aggregate)
        {
            var category = await _categoryService.GetAsync(aggregate.CategoryId);
            aggregate.Category = _mapper.Map<CategoryDto>(category);
            
            return aggregate;
        }

        private async Task UpdateCachedProductList(CreateProductDto dto)
        {
            var dtos = await _cacheManager.GetAsync<List<CreateProductDto>>(nameof(GetAllProductsQuery));

            if(dtos is not null)
            {
                dtos.Add(dto);

                await _cacheManager.SetExpireAsync(nameof(GetAllProductsQuery), dtos, TimeSpan.FromMinutes(5));
            } 
        }
    }
}
