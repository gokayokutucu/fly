using AutoMapper;
using Fly.Application.DomainEvents.Products.Dtos;
using Fly.Common;
using Fly.Domain.Aggreagates;
using MediatR;

namespace Fly.Application.DomainEvents.Products.Queries.GetProducts
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
    {
        private readonly ICacheManager _cacheManager;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;


        public GetAllProductsQueryHandler(ICacheManager cacheManager, IMapper mapper, IProductService productService, ICategoryService categoryService)
        {
            _cacheManager = cacheManager;
            _mapper = mapper;
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var keyName = nameof(GetAllProductsQuery);
            List<ProductDto> dtos = await _cacheManager.GetAsync<List<ProductDto>>(keyName);

            var count = await _productService.CountAsync();

            if (dtos is null || dtos?.Count != count)
                dtos = await GetProducts(keyName);

            return dtos;

        }

        private async Task<List<ProductDto>> GetProducts(string keyName)
        {
            try
            {
                var entityProducts = await _productService.GetAsync();

                var productDtos = new List<ProductDto>();

                await Parallel.ForEachAsync(entityProducts, async (entityProduct, cancellationToken) =>
                {
                    var entityCategory = await _categoryService.GetAsync(entityProduct.CategoryId);

                    var category = _mapper.Map<CategoryDto>(entityCategory);
                    productDtos.Add(new ProductDto
                    {
                        Id = entityProduct.Id,
                        Name = entityProduct.Name,
                        Description = entityProduct.Description,
                        Currency = entityProduct.Currency,
                        Price = entityProduct.Price,
                        CategoryId = category.Id,
                        Category = category
                    });
                });

                if(productDtos.Any())
                    await _cacheManager.SetExpireAsync(keyName, productDtos, TimeSpan.FromMinutes(5));

                return productDtos;
            }
            catch (Exception)
            {
                throw;
            }           
        }
    }
}
