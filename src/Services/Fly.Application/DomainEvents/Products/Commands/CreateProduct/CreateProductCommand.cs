using Fly.Application.DomainEvents.Products.Dtos;
using MediatR;

namespace Fly.Application.DomainEvents.Products.Commands.AddProduct
{
    public class CreateProductCommand : IRequest<string>
    {
        public ProductDto ProductDto { get; set; }
    }
}
