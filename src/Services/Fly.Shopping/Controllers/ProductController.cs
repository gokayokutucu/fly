using AutoMapper;
using Fly.Application.DomainEvents.Products.Commands.AddProduct;
using Fly.Application.DomainEvents.Products.Commands.DeleteProduct;
using Fly.Application.DomainEvents.Products.Commands.UpdateProduct;
using Fly.Application.DomainEvents.Products.Dtos;
using Fly.Application.DomainEvents.Products.Queries.GetProduct;
using Fly.Application.DomainEvents.Products.Queries.GetProducts;
using Fly.Application.DomainEvents.Products.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fly.Shopping.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<ProductController> _logger;

        public ProductController( IMapper mapper, IMediator mediator, ILogger<ProductController> logger)
        {
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<GetProductViewModel>> GetAsync()
        {
            var dtos = await _mediator.Send(new GetAllProductsQuery());
            var result = _mapper.Map<List<GetProductViewModel>>(dtos);
            return result;
        } 

        [HttpGet("{id:length(24)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<GetProductViewModel> GetAsync(string id) => _mapper.Map<GetProductViewModel>(await _mediator.Send(new GetProductByIdQuery()
        {
            Id = id
        }));

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post(PostProductViewModel model)
        {
            var notification = new CreateProductNotification()
            {
                ProductDto = _mapper.Map<CreateProductDto>(model)
            };
            
            await _mediator.Publish(notification);
                
            return Ok(notification.ProductDto.Id);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Update(PutProductViewModel model)
        {
            var product = await _mediator.Send(new GetProductByIdQuery()
            {
                Id = model.Id
            });

            if (product is null)
            {
                return NotFound();
            }

            await _mediator.Send(new UpdateProductCommand()
            {
                ProductDto = _mapper.Map<ProductDto>(model)
            });

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(string id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery()
            {
                Id = id
            });

            if (product is null)
            {
                return NotFound();
            }

            await _mediator.Send(new DeleteProductCommand()
            {
                Id = product.Id
            });

            return NoContent();
        }
    }
}