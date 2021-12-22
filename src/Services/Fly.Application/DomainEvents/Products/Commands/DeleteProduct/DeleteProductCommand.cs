using MediatR;

namespace Fly.Application.DomainEvents.Products.Commands.DeleteProduct
{
    public class DeleteProductCommand : IRequest<Unit>
    {
        public string Id { get; set; }
    }
}
