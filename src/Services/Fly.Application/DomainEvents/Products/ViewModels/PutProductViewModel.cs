using AutoMapper;
using Fly.Application.Common.Mappings;
using Fly.Application.DomainEvents.Products.Dtos;

namespace Fly.Application.DomainEvents.Products.ViewModels
{
    public class PutProductViewModel : IMapFrom<ProductDto>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ProductDto, PutProductViewModel>().ReverseMap();
        }
    }
}
