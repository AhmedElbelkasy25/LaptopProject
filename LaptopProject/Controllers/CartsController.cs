using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Threading.Tasks;

namespace LaptopProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public CartsController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            this._userManager = userManager;
            this._unitOfWork = unitOfWork;
        }
        [Authorize]
        [HttpPost("{productId}")]
        public async Task<IActionResult> Create([FromRoute] int productId, [FromQuery] int count, CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {

                await _unitOfWork.CartRepository.CreateAsync(new Cart()
                {
                    ProductId = productId,
                    ApplicationUserId = userId,
                    Count = count
                }, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
                return Created();
            }
            return NotFound();
        }
        [Authorize]
        [HttpGet("")]
        public IActionResult GetAll()
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var carts = _unitOfWork.CartRepository.Get(filter: e => e.ApplicationUserId == userId, includeProps: e => e.Include(e => e.Product)).ToList();
                var returnedCart = carts.Select(e => e.Product).Adapt<List<CartResponse>>();
                return Ok(new
                {
                    returnedCart.Count,
                    returnedCart
                });
            }
            return NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete( [FromRoute] int id, CancellationToken cancellationToken)
        {
            var cart = _unitOfWork.CartRepository.GetOne(e => e.Id == id);
            if (cart != null)
            {
                _unitOfWork.CartRepository.Delete(cart);
                _unitOfWork.CommitAsync(cancellationToken);
                return NoContent();
            }
            return NotFound();
        }

        [Authorize]
        [HttpGet("Pay")]
        public IActionResult Pay()
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                

            };

            var applicationUserId = _userManager.GetUserId(User);
            var shoppingCarts = _unitOfWork.CartRepository.Get(
               e => e.ApplicationUserId == applicationUserId ,
               e=>e.Include(e=>e.Product)).ToList();


            foreach (var item in shoppingCarts)
            {

                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                            Description = item.Product.Description,
                        },

                        UnitAmount = (long)item.Product.Price * 100,
                    },
                    Quantity = item.Count
                });
            }

            var service = new SessionService();
            var session = service.Create(options);
            return Ok(session.Url);
        }


       
    }
}
