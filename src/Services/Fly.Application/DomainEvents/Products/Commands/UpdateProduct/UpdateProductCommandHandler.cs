using AutoMapper;
using Fly.Application.DomainEvents.Products.Dtos;
using Fly.Application.DomainEvents.Products.Queries.GetProducts;
using Fly.Application.Exceptions;
using Fly.Common;
using Fly.Domain.Aggreagates;
using Fly.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fly.Application.DomainEvents.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly ILogger _logger;
        private readonly ICacheManager _cacheManager;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public UpdateProductCommandHandler(
            ILogger<UpdateProductCommandHandler> logger, 
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

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    return Unit.Value;

                var entity = _mapper.Map<Product>(request.ProductDto);

                var keyName = $"product_{entity.Id}";

                await _productService.UpdateAsync(entity);

                var dto = await SetDtoAsync(entity);

                if (dto is not null)
                    await _cacheManager.SetExpireAsync(keyName, _mapper.Map<ProductDto>(request.ProductDto), TimeSpan.FromMinutes(5));

                await UpdateCachedProductList(dto);

                return Unit.Value;
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

            var item = dtos.SingleOrDefault(x => x.Id == dto.Id);

            if(item != null)
            {
                dtos.Remove(item);

                dtos.Add(dto);

                await _cacheManager.SetExpireAsync(nameof(GetAllProductsQuery), dtos, TimeSpan.FromMinutes(5));
            }               
        }
    }
}
