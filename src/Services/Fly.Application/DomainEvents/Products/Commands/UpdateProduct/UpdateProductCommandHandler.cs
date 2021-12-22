using AutoMapper;
using Fly.Application.DomainEvents.Products.Dtos;
using Fly.Application.Exceptions;
using Fly.Common;
using Fly.Common.Extensions;
using Fly.Domain.Aggreagates;
using Fly.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Fly.Application.DomainEvents.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly ILogger _logger;
        private readonly ICacheManager _cacheManager;
        private readonly IMapper _mapper;
        private readonly IProductService _service;

        public UpdateProductCommandHandler(ILogger<UpdateProductCommandHandler> logger, ICacheManager cacheManager, IMapper mapper, IProductService service)
        {
            _logger = logger;
            _cacheManager = cacheManager;
            _mapper = mapper;
            _service = service;
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    return Unit.Value;

                var entity = _mapper.Map<Product>(request.ProductDto);

                var keyName = $"product_{entity.Id}";

                await _service.UpdateAsync(entity);

                //await _distributedCache.SetAsync(keyName, request.ProductDto, cancellationToken);
                await _cacheManager.SetExpireAsync(keyName, _mapper.Map<ProductDto>(request.ProductDto), TimeSpan.FromMinutes(5));

                return Unit.Value;
            }
            catch (FlyException ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }

            
        }
    }
}
