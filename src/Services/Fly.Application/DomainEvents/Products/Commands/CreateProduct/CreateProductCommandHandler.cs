﻿using AutoMapper;
using Fly.Application.DomainEvents.Products.Dtos;
using Fly.Application.Exceptions;
using Fly.Common;
using Fly.Common.Extensions;
using Fly.Domain.Aggreagates;
using Fly.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Fly.Application.DomainEvents.Products.Commands.AddProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Unit>
    {
        private readonly ILogger _logger;
        private readonly ICacheManager _cacheManager;
        private readonly IMapper _mapper;
        private readonly IProductService _service;

        public CreateProductCommandHandler(
            ILogger<CreateProductCommandHandler> logger,
            ICacheManager cacheManager,
            IMapper mapper, 
            IProductService service)
        {
            _logger = logger;
            _cacheManager = cacheManager;
            _mapper = mapper;
            _service = service;
        }

        public async Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    return Unit.Value;

                var entity = _mapper.Map<Product>(request.ProductDto);

                await _service.CreateAsync(entity);

                var keyName = $"product_{entity.Id}";

                //await _distributedCache.SetAsync(keyName, _mapper.Map<ProductDto>(request.ProductDto), cancellationToken);

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
