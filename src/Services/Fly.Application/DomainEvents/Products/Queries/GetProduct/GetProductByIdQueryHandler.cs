using AutoMapper;
using Fly.Application.DomainEvents.Products.Dtos;
using Fly.Common;
using Fly.Common.Extensions;
using Fly.Domain.Aggreagates;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Fly.Application.DomainEvents.Products.Queries.GetProduct
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly ICacheManager _cacheManager;
        private readonly IDistributedCache _distributedCache;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public GetProductByIdQueryHandler(
            ICacheManager cacheManager,
            IDistributedCache distributedCache, 
            IMapper mapper, 
            IProductService productService, 
            ICategoryService categoryService)
        {
            _cacheManager = cacheManager;
            _distributedCache = distributedCache;
            _mapper = mapper;
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var keyName = $"product_{request.Id}";
            //ProductDto dto = await _distributedCache.GetAsync<ProductDto>(keyName)
            //    ?? await GetProduct(keyName, request, cancellationToken);
            ProductDto dto = await _cacheManager.GetAsync<ProductDto>(keyName)
                ?? await GetProduct(keyName, request, cancellationToken);

            return dto;
        }

        private async Task<ProductDto> GetProduct(string keyName, GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var entityProduct = await _productService.GetAsync(request.Id);
            var entityCategory = await _categoryService.GetAsync(entityProduct.CategoryId);

            var dto = new ProductDto()
            {
                Id = entityProduct.Id,
                Name = entityProduct.Name,
                Description = entityProduct.Description,
                Currency = entityProduct.Currency,
                Price = entityProduct.Price,
                Category = _mapper.Map<CategoryDto>(entityCategory)
            };

            await _cacheManager.SetExpireAsync(keyName, dto, TimeSpan.FromMinutes(5));

            return dto;
        }
    }
}
