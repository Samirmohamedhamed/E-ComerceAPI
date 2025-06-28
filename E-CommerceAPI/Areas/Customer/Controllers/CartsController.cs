using E_CommerceAPI.DtosConfiguration.DtoResponse;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace E_CommerceAPI.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize]
    public class CartsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IProductRepository productRepository;
        private readonly ICartRepository cartRepository;

        public CartsController(UserManager<ApplicationUser> userManager, IProductRepository productRepository, ICartRepository cartRepository)
        {
            this.userManager = userManager;
            this.productRepository = productRepository;
            this.cartRepository = cartRepository;
        }
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromQuery] int productId, [FromQuery] int count)
        {
            var user = await userManager.GetUserAsync(User);
            ModelStateDictionary keyValuePairs = new ModelStateDictionary();
            if (user is not null)
            {
                var product = productRepository.GetOne(e => e.Id == productId);
                if (product is not null)
                {
                    var cartInDb = cartRepository.GetFirstOrDefault(e => e.ProductId == productId && e.ApplicationId == user.Id);
                    if (cartInDb is not null)
                    {
                        cartInDb.Count += count;
                    }
                    else
                    {
                        if (count <= product.Quantity && count > 0)
                        {
                            await cartRepository.CreateAsync(new()
                            {
                                ProductId = productId,
                                ApplicationId = user.Id,
                                Count = count,
                            });
                        }
                        else
                        {
                            keyValuePairs.AddModelError(string.Empty, "Quantity Less Than The Count");
                            return BadRequest(keyValuePairs);
                        }
                    }

                    await cartRepository.CommitAsync();

                    return Ok();
                }
                keyValuePairs.AddModelError(string.Empty, "User Is Null");
                return Unauthorized(keyValuePairs);
                

            }
            return BadRequest();

        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                var carts = cartRepository.Get(expression: e => e.ApplicationId == user.Id, Includes: [e => e.Product]);
                var total = carts.Sum(e => e.Product.Price * e.Count);

                var response = new CartWithTotalPriceResponse()
                {
                    cartResponse = carts.Adapt<IEnumerable<CartResponse>>(),
                    totalPrice = total,
                };

                return Ok(response);  
            }
            return Unauthorized();
        }

        [HttpGet("decrementValue")]

        public async Task<IActionResult> decrement([FromQuery]int productId)
        {
            var user = await userManager.GetUserAsync(User);
            var cartInDb = cartRepository.GetFirstOrDefault(e => e.ProductId == productId && e.ApplicationId == user.Id);
            if (cartInDb is not null)
            {
                cartInDb.Count--;
                await cartRepository.CommitAsync();
                return Ok();

            }
            return BadRequest();


        }
        [HttpGet("DeleteItemFromCart")]

        public async Task<IActionResult> DeleteItemFromCart([FromQuery]int productId)
        {
            var user = await userManager.GetUserAsync(User);
            var cartInDb = cartRepository.GetFirstOrDefault(e => e.ProductId == productId && e.ApplicationId == user.Id);
            if (cartInDb is not null)
            {
              await  cartRepository.DeleteAsync(cartInDb);
                await cartRepository.CommitAsync();
                return Ok();

            }
            return BadRequest();


        }


    }
}
