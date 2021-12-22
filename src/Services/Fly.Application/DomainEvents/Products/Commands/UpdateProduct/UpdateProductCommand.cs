using Fly.Application.DomainEvents.Products.Dtos;
using MediatR;

namespace Fly.Application.DomainEvents.Products.Commands.UpdateProduct
{
    public class UpdateProductCommand : IRequest<Unit>
    {
        public ProductDto ProductDto;
    }
}
