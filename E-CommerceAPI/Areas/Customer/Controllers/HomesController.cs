using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace E_CommerceAPI.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class HomesController : ControllerBase
    {
        private readonly IProductRepository productRepository;
        private readonly ICategoryRepository categoryRepository;

        public HomesController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
        }

        [HttpGet]
        public IActionResult GetProducts(
            int categoryId = 0,
            string? query = null,
            double minPrice = 0,
            double maxPrice = 0,
            int pageNumber = 1)
        {
            var categories = categoryRepository.Get().ToList();

            Expression<Func<Product, bool>> filter = p => true;

            if (categoryId > 0)
                filter = Combine(filter, p => p.CategoryId == categoryId);

            if (!string.IsNullOrWhiteSpace(query))
                filter = Combine(filter, p => p.Name.Contains(query));

            if (minPrice > 0)
                filter = Combine(filter, p => p.Price >= (decimal)minPrice);

            if (maxPrice > 0)
                filter = Combine(filter, p => p.Price <= (decimal)maxPrice);

            int pageSize = 8;
            int skip = (pageNumber - 1) * pageSize;

            var products = productRepository.Get(
                expression: filter,
                Includes: new[] { (Expression<Func<Product, object>>)(p => p.Category) },
                skip: skip,
                take: pageSize
            );

            var totalCount = productRepository.Get(expression: filter).Count();

            return Ok(new
            {
                Categories = categories,
                Products = products,
                Pagination = new
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalItems = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                }
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductDetails(int id)
        {
            var product = productRepository.GetOne(p => p.Id == id);

            if (product == null)
                return NotFound(new { Message = "Product not found." });

            var relatedProduct = productRepository.Get(
                expression: p => p.Name.Contains(product.Name.Substring(0, 5)) && p.Id != product.Id,
                take: 4
            );

            var relatedCategory = productRepository.Get(
                expression: p => p.CategoryId == product.CategoryId && p.Id != product.Id,
                Includes: new[] { (Expression<Func<Product, object>>)(p => p.Category) },
                skip: 1,
                take: 4
            );

            var topProduct = productRepository
                .Get(expression: p => p.Id != product.Id)
                .OrderByDescending(p => p.Traffic)
                .Take(4)
                .ToList();

            product.Traffic++;
          await  productRepository.UpdateAsync(product);
            await productRepository.CommitAsync();

            var viewModel = new
            {
                Product = product,
                RelatedProducts = relatedProduct.ToList(),
                RelatedCategory = relatedCategory.ToList(),
                TopProducts = topProduct
            };

            return Ok(viewModel);
        }

        // Helper to combine two expressions
        private Expression<Func<Product, bool>> Combine(Expression<Func<Product, bool>> expr1, Expression<Func<Product, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(Product));

            var body = Expression.AndAlso(
                Expression.Invoke(expr1, parameter),
                Expression.Invoke(expr2, parameter)
            );

            return Expression.Lambda<Func<Product, bool>>(body, parameter);
        }
    }
}
