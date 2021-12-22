using AutoMapper;
using Fly.Application.DomainEvents.Products.Dtos;
using Fly.Application.Exceptions;
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
        private readonly IDistributedCache _distributedCache;
        private readonly IMapper _mapper;
        private readonly IProductService _service;

        public UpdateProductCommandHandler(ILogger<UpdateProductCommandHandler> logger, IDistributedCache distributedCache, IMapper mapper, IProductService service)
        {
            _logger = logger;
            _distributedCache = distributedCache;
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

                await _distributedCache.SetAsync(keyName, request.ProductDto, cancellationToken);

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
