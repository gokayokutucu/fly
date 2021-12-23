using AutoMapper;
using Fly.Application.Common.Mappings;
using Fly.Domain.Entities;
namespace Fly.Application.DomainEvents.Products.Dtos
{
    public class ProductDto : IMapFrom<Product>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }
        public CategoryDto Category { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public long Version { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Category, CategoryDto>().ReverseMap();
            profile.CreateMap<Product, ProductDto>().ReverseMap();
        }

    }
}
