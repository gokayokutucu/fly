using Fly.Domain.Aggreagates;
using Fly.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Fly.Shopping.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<List<Product>> GetAsync() =>
       (List<Product>)await _productService.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<Product> GetAsync(string id)
        {
            return await _productService.GetAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Product data)
        {
            await _productService.CreateAsync(data);

            return Ok(data.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Update(Product data)
        {
            var product = await _productService.GetAsync(data.Id);

            if (product is null)
            {
                return NotFound();
            }

            await _productService.UpdateAsync(data);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var product = await _productService.GetAsync(id);

            if (product is null)
            {
                return NotFound();
            }

            await _productService.DeleteAsync(id);

            return NoContent();
        }
    }
}