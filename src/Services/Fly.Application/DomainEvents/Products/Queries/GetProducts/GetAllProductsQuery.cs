using Fly.Application.DomainEvents.Products.Dtos;
using MediatR;

namespace Fly.Application.DomainEvents.Products.Queries.GetProducts
{
    public class GetAllProductsQuery : IRequest<List<ProductDto>>
    {
    }
}
