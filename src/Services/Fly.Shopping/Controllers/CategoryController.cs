using Fly.Domain.Aggreagates;
using Fly.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Planet.MongoDbCore;

namespace Fly.Shopping.Controllers
{
    [ApiController]
    [Route("categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;
        private readonly IConfiguration _config;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger, IConfiguration config)
        {
            _categoryService = categoryService;
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<List<Category>> GetAsync() => await _categoryService.GetAsync();

        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<dynamic> GetAsync()
        //{
        //    var conf = _config.GetSection("ConnectionStrings")
        //        .GetSection("MongoDbConnection")
        //        .Get<MongoDbContextConfiguration>();
        //    return new
        //    {
        //        Url = conf.Url,
        //        Cache = _config["CacheConnection"].ToString()
        //    };
        //}

        [HttpGet("{id:length(24)}")]
        public async Task<Category> GetAsync(string id)
        {
            return await _categoryService.GetAsync(id);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post(Category data)
        {
            await _categoryService.CreateAsync(data);

            return Ok(data.Id);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Category data)
        {
            var category = await _categoryService.GetAsync(data.Id);

            if (category is null)
            {
                return NotFound();
            }

            await _categoryService.UpdateAsync(data);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string id)
        {
            var category = await _categoryService.GetAsync(id);

            if (category is null)
            {
                return NotFound();
            }

            await _categoryService.DeleteAsync(id);

            return NoContent();
        }
    }
}