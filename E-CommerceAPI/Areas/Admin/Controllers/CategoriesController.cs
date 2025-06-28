using E_CommerceAPI.DtosConfiguration.DtoRequest;
using E_CommerceAPI.DtosConfiguration.DtoResponse;
using E_CommerceAPI.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace E_CommerceAPI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/Admin/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = categoryRepository.Get();
            return Ok(categories.Adapt<IEnumerable<CategoryResponse>>());
        }
        [HttpGet("{id}")]
        public IActionResult GetOne(int id)
        {
            var category = categoryRepository.GetFirstOrDefault(e => e.Id == id);

            return Ok(category.Adapt<CategoryResponse>());
        }
        [HttpPost]
        public async Task<IActionResult> Create(CategoryRequest categoryRequest)
        {

           var category= categoryRequest.Adapt<Category>();
            var newCategory = await categoryRepository.CreateAsync(category);
            if(newCategory is not null)
            {
                await categoryRepository.CommitAsync();
                return Created($"{Request.Scheme}://{Request.Host}/api/Categories/{newCategory.Id}", newCategory);

            }
            return BadRequest();

        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] CategoryRequest categoryRequest)
        {
            var categoryDb = categoryRepository.GetFirstOrDefault(e => e.Id == id);

            if (categoryRequest == null || categoryDb == null)
                return BadRequest("Invalid category data or ID not found.");

            categoryDb.Name = categoryRequest.Name;
            categoryDb.Description = categoryRequest.Description;
            categoryDb.Status = categoryRequest.Status;

            await categoryRepository.CommitAsync();

            return Ok("Category updated successfully.");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = categoryRepository.GetFirstOrDefault(e => e.Id == id);
            if (category is not null)
            {
                var Result = await categoryRepository.DeleteAsync(category);

                if (Result is not null)
                {
                    await categoryRepository.CommitAsync();
                    return NoContent(); ;
                }
            }
            return NotFound();

        }
    }
}
