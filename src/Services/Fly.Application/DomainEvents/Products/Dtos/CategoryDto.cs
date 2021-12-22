using AutoMapper;
using Fly.Application.Common.Mappings;
using Fly.Domain.Entities;

namespace Fly.Application.DomainEvents.Products.Dtos
{
    public class CategoryDto : IMapFrom<Category>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Category, CategoryDto>();
        }
    }
}
