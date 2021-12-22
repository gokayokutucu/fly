using AutoMapper;
using Fly.Application.DomainEvents.Products.Dtos;
using Fly.Domain.Aggreagates;
using MediatR;

namespace Fly.Application.DomainEvents.Products.Queries.GetProducts
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public GetAllProductsQueryHandler(IMapper mapper, IProductService productService, ICategoryService categoryService)
        {
            _mapper = mapper;
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
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
                    Category = category
                });
            });

            return productDtos;
        }
    }
}
