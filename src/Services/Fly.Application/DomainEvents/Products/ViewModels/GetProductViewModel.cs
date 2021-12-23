using AutoMapper;
using Fly.Application.Common.Mappings;
using Fly.Application.DomainEvents.Products.Dtos;

namespace Fly.Application.DomainEvents.Products.ViewModels
{
    public class GetCategoryViewModel : IMapFrom<CategoryDto>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CategoryDto, GetCategoryViewModel>().ReverseMap();
        }
    }

    public class GetProductViewModel : IMapFrom<ProductDto>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }
        public GetCategoryViewModel Category { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CategoryDto, GetCategoryViewModel>().ReverseMap();
            profile.CreateMap<ProductDto, GetProductViewModel>().ReverseMap();
        }

    }
}
