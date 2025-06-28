using E_CommerceAPI.DtosConfiguration.DtoResponse;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceAPI.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize]

    public class FavoriteController : ControllerBase
    {
        private readonly IProductRepository productRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IFavoritesRepository favoritesRepository;

        public FavoriteController(
            IProductRepository productRepository,
            UserManager<ApplicationUser> userManager,
            IFavoritesRepository favoritesRepository)
        {
            this.productRepository = productRepository;
            this.userManager = userManager;
            this.favoritesRepository = favoritesRepository;
        }

        [HttpPost("Add/{id}")]
        public async Task<IActionResult> AddToFavorites(int id)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { Message = "User not authenticated." });

            var product = productRepository.GetOne(e => e.Id == id);
            if (product == null)
                return NotFound(new { Message = "Product not found." });

            var favorite = new Favorite
            {
                ProductId = id,
                ApplicationId = user.Id,
                CreatedAt = DateTime.UtcNow,
            };

            await favoritesRepository.CreateAsync(favorite);
            await favoritesRepository.CommitAsync();

            return Ok(new { Message = "Product added to favorites successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var user = await userManager.GetUserAsync(User);
            if (user is not null)
            {
                var favorite = favoritesRepository.Get(e => e.ApplicationId == user.Id, Includes: [e => e.Product]).ToList();
                
                return Ok(favorite.Adapt<IEnumerable<CartResponse>>());
            }
            return BadRequest();
        }
        [HttpDelete("Delete/{productId}")]
        public async Task<IActionResult> DeleteItemFromFavorite(int productId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(new { Message = "User not authenticated." });

            var favorite = favoritesRepository.GetFirstOrDefault(e => e.ApplicationId == user.Id && e.ProductId == productId);
            if (favorite == null)
                return NotFound(new { Message = "Favorite item not found." });

           await favoritesRepository.DeleteAsync(favorite);
            await favoritesRepository.CommitAsync();

            return Ok(new { Message = "Favorite item deleted successfully." });
        }
    }

}

