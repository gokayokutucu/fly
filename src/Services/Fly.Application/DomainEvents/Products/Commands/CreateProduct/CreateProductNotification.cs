using Fly.Application.DomainEvents.Products.Dtos;
using MediatR;

namespace Fly.Application.DomainEvents.Products.Commands.AddProduct
{
    public class CreateProductNotification : INotification
    {
        public CreateProductDto ProductDto { get; set; }
    }
}
