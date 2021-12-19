using Fly.Domain.Entities;
using Fly.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fly.Shopping.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
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
        public IEnumerable<WeatherForecast> Get(string id)
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        public async Task<IActionResult> Post(Product data)
        {
            await _productService.CreateAsync(data);

            return CreatedAtAction(nameof(Get), new { id = data.Id }, data);
        }
    }
}