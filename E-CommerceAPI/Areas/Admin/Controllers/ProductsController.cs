using E_CommerceAPI.DtosConfiguration.DtoRequest;
using E_CommerceAPI.DtosConfiguration.DtoResponse;
using E_CommerceAPI.Repositories;
using E_CommerceAPI.Repositories.IRepositories;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace E_CommerceAPI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/Admin/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var products = productRepository.Get(Includes: [e => e.Category, e => e.Brand]);//_context.Products.Include(e => e.Category).Include(e => e.Brand)

            return Ok(products.Adapt<IEnumerable<ProductResponse>>());
        }
        [HttpGet("{id}")]
        public IActionResult GetOne(int id)
        {
            var product = productRepository.GetFirstOrDefault(e => e.Id == id, Includes: [e => e.Category, e => e.Brand]);//_context.Products.Include(e => e.Category).Include(e => e.Brand)

            return Ok(product.Adapt<ProductResponse>());
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductRequest productRequest)
        {


            if (productRequest.MainImg != null && productRequest.MainImg.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(productRequest.MainImg.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Images"
                    , fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    productRequest.MainImg.CopyTo(stream);
                }
                var Product = productRequest.Adapt<Product>();

                Product.MainImg = fileName;
                var newProduct = await productRepository.CreateAsync(Product);
                if (newProduct is not null)
                {
                    await productRepository.CommitAsync();
                    return Created($"{Request.Scheme}://{Request.Host}/api/Brands/{newProduct.Id}", newProduct);
                }
            }
            return BadRequest();

        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] ProductRequest productRequest)
        {
            var ProductDb = productRepository.GetFirstOrDefault(e => e.Id == id, traked: false);
            var Product = productRequest.Adapt<Product>();


            if (ProductDb == null)
                return NotFound("Product not found.");
            Product.Id = ProductDb.Id;

            if (productRequest.MainImg != null && productRequest.MainImg.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(productRequest.MainImg.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Images"
                    , fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await productRequest.MainImg.CopyToAsync(stream);
                }
                var oldFileName = ProductDb.MainImg;
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "Images"
                    , ProductDb.MainImg);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
                Product.MainImg = fileName;


            }
            else
            {
                Product.MainImg = ProductDb.MainImg;
            }
            var newProduct = await productRepository.UpdateAsync(Product);
            if (newProduct is not null)
            {
                await productRepository.CommitAsync();
                return Created($"{Request.Scheme}://{Request.Host}/api/Products/{newProduct.Id}", newProduct);
            }

            return BadRequest("Failed to update product.");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var productInDb = productRepository.GetFirstOrDefault(e => e.Id == id);
            if (productInDb is not null)
            {
                if (productInDb.MainImg is not null)
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "Images"
                , productInDb.MainImg);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }
                var Result = await productRepository.DeleteAsync(productInDb);

                if (Result is not null)
                {
                    await productRepository.CommitAsync();
                    return NoContent(); ;
                }
            }
            return NotFound();

        }
        [HttpGet("DeleteImage/{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var product =productRepository.GetFirstOrDefault(expression:e=>e.Id==id);
            if (product is not null)
            {
                var oldFileName = product.MainImg;
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"
                    , oldFileName);
                Console.WriteLine(oldPath);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
                product.MainImg = "dummyImage.png";
             await  productRepository.UpdateAsync(product);
             await   productRepository.CommitAsync();
                return Ok();
            }
            return NotFound();

        }
    }
}
