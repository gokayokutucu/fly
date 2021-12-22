using Fly.Domain.Aggreagates;
using MediatR;

namespace Fly.Application.DomainEvents.Products.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly IProductService _service;

        public DeleteProductCommandHandler(IProductService service)
        {
            _service = service;
        }

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            if(cancellationToken.IsCancellationRequested)
                return Unit.Value;

            await _service.DeleteAsync(request.Id);

            return Unit.Value;
        }
    }
}
