using Fly.Application.DomainEvents.Products.Dtos;
using Fly.Application.DomainEvents.Products.Queries.GetProducts;
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

                await UpdateCachedProductList(request.Id);

                return Unit.Value;
            }
            catch (Exception)
            {
                throw;
            }            
        }

        private async Task UpdateCachedProductList(string id)
        {
            var dtos = await _cacheManager.GetAsync<List<ProductDto>>(nameof(GetAllProductsQuery));

            var item = dtos.SingleOrDefault(x => x.Id == id);

            if (item != null)
            {
                dtos.Remove(item);

                await _cacheManager.SetExpireAsync(nameof(GetAllProductsQuery), dtos, TimeSpan.FromMinutes(5));
            }
        }
    }
}
