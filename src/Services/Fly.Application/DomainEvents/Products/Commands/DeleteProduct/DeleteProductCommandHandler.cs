using Fly.Common;
using Fly.Domain.Aggreagates;
using MediatR;

namespace Fly.Application.DomainEvents.Products.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly IProductService _service;
        private readonly ICacheManager _cacheManager;

        public DeleteProductCommandHandler(ICacheManager cacheManager, IProductService service)
        {
            _cacheManager = cacheManager;
            _service = service;
        }

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    return Unit.Value;

                await _service.DeleteAsync(request.Id);

                var keyName = $"product_{request.Id}";
                await _cacheManager.DeleteAsync(keyName);

                return Unit.Value;
            }
            catch (Exception)
            {
                throw;
            }            
        }
    }
}
