using Fly.Application.DomainEvents.Products.Dtos;
using MediatR;

namespace Fly.Application.DomainEvents.Products.Queries.GetProduct
{
    public class GetProductByIdQuery : IRequest<ProductDto>
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
