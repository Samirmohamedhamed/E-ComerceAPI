using E_CommerceAPI.DtosConfiguration.DtoRequest;
using E_CommerceAPI.DtosConfiguration.DtoResponse;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceAPI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/Admin/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandRepository brandRepository;

        public BrandsController(IBrandRepository brandRepository)
        {
            this.brandRepository = brandRepository;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = brandRepository.Get();
            return Ok(categories.Adapt<IEnumerable<BrandResponse>>());
        }
        [HttpGet("{id}")]
        public IActionResult GetOne(int id)
        {
            var brand = brandRepository.GetFirstOrDefault(e => e.Id == id);

            return Ok(brand.Adapt<BrandResponse>());
        }
        [HttpPost]
        public async Task<IActionResult> Create(BrandRequest brandRequest)
        {

           var brand= brandRequest.Adapt<Brand>();
            var newBrand = await brandRepository.CreateAsync(brand);
            if(newBrand is not null)
            {
                await brandRepository.CommitAsync();
                return Created($"{Request.Scheme}://{Request.Host}/api/Brands/{newBrand.Id}", newBrand);

            }
            return BadRequest();

        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] BrandRequest brandRequest)
        {
            var brandDb = brandRepository.GetFirstOrDefault(e => e.Id == id);

            if (brandRequest == null || brandDb == null)
                return BadRequest("Invalid brand data or ID not found.");

            brandDb.Name = brandRequest.Name;
            brandDb.Description = brandRequest.Description;
            brandDb.Status = brandRequest.Status;

            await brandRepository.CommitAsync();

            return Ok("Brand updated successfully.");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var brand = brandRepository.GetFirstOrDefault(e => e.Id == id);
            if (brand is not null)
            {
                var Result = await brandRepository.DeleteAsync(brand);

                if (Result is not null)
                {
                    await brandRepository.CommitAsync();
                    return NoContent(); ;
                }
            }
            return NotFound();

        }
    }
}
