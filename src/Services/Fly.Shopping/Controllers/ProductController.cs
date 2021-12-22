using Fly.Application.DomainEvents.Products.Commands.DeleteProduct;
using Fly.Application.DomainEvents.Products.Commands.UpdateProduct;
using Fly.Application.DomainEvents.Products.Dtos;
using Fly.Application.DomainEvents.Products.Queries.GetProduct;
using Fly.Application.DomainEvents.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fly.Shopping.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IMediator mediator, ILogger<ProductController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<ProductDto>> GetAsync() => await _mediator.Send(new GetAllProductsQuery());

        [HttpGet("{id:length(24)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ProductDto> GetAsync(string id) => await _mediator.Send(new GetProductByIdQuery()
        {
            Id = id
        });

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Post(ProductDto data)
        {
            await _mediator.Send(data);
            return Ok(data.Id);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Update(ProductDto data)
        {
            var product = await _mediator.Send(new GetProductByIdQuery()
            {
                Id = data.Id
            });

            if (product is null)
            {
                return NotFound();
            }

            await _mediator.Send(new UpdateProductCommand()
            {
                ProductDto = data
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