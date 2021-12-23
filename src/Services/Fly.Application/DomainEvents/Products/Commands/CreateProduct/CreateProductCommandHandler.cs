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
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, string>
    {
        private readonly ILogger _logger;
        private readonly ICacheManager _cacheManager;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public CreateProductCommandHandler(
            ILogger<CreateProductCommandHandler> logger,
            ICacheManager cacheManager,
            IMapper mapper,
            IProductService productService,
            ICategoryService categoryService)
        {
            _logger = logger;
            _cacheManager = cacheManager;
            _mapper = mapper;
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<string> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    return default;

                var entity = _mapper.Map<Product>(request.ProductDto);

                await _productService.CreateAsync(entity);

                var dto = await SetDtoAsync(entity);

                var keyName = $"product_{entity.Id}";

                if (dto is not null)
                    await _cacheManager.SetExpireAsync(keyName, dto, TimeSpan.FromMinutes(5));

                await UpdateCachedProductList(dto);

                return entity.Id;
            }
            catch (FlyException ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }

        }

        private async Task<ProductDto> SetDtoAsync(Product product)
        {
            var dto = _mapper.Map<ProductDto>(product);
            var category = await _categoryService.GetAsync(product.CategoryId);
            dto.Category = _mapper.Map<CategoryDto>(category);
            return dto;
        }

        private async Task UpdateCachedProductList(ProductDto dto)
        {
            var dtos = await _cacheManager.GetAsync<List<ProductDto>>(nameof(GetAllProductsQuery));

            if(dtos is not null)
            {
                dtos.Add(dto);

                await _cacheManager.SetExpireAsync(nameof(GetAllProductsQuery), dtos, TimeSpan.FromMinutes(5));
            } 
        }
    }
}
