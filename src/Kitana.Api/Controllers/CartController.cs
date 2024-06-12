using Kitana.Api.Middleware;
using Kitana.Service.Model;
using Kitana.Service.Model.ResponseHandlers;
using Kitana.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scaf.Models;
using System.Threading.Tasks;


namespace Kitana.Api.Controllers
{
    /// <summary>
    /// Controller for managing the user's shopping cart.
    /// </summary>
    [Route("cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartController"/> class.
        /// </summary>
        /// <param name="cartService">The service responsible for cart operations.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="cartService"/> is <c>null</c>.</exception>
        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// Retrieves the user's cart items.
        /// </summary>
        /// 200 OK - Success.
        /// <response code="400"> Bad Request </response> 
        /// <response code="500"> Internal Server Error </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="403"> Forbidden </response>
        /// <response code="404"> Not Found </response>
        [Permissions("EndUser")]
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var cartitems = await _cartService.GetCartItemsAsync();
            return Ok(ResponseHandler.HandleSuccess("Successfully fetched cart items", cartitems));
        }

        /// <summary>
        /// Adds a new item to the user's cart.
        /// </summary>
        /// <param name="cartItemRQ">
        /// - courseId : A unique string representing the course identifier.
        /// - discount : Set the discount applied to the course price.
        /// </param>
        /// 200 OK - Success.
        /// <response code="400"> Bad Request </response> 
        /// <response code="500"> Internal Server Error </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="403"> Forbidden </response>
        /// <response code="404"> Not Found </response>
        [Permissions("EndUser")]
        [HttpPost("items")]
        public async Task<IActionResult> AddCartItem(CartItemRQ cartItemRQ)
        {
            var addedCartItem = await _cartService.AddCartItemAsync(cartItemRQ);
            return Ok(ResponseHandler.HandleSuccess("Successfully added course to cart", addedCartItem));
        }

        /// <summary>
        /// Updates existing cart items.
        /// </summary>
        /// <param name="cartItemRQ">The request object containing updated cart item details.
        /// - courseId : A unique string representing the course identifier.
        /// - discount : Set the discount applied to the course price.
        /// </param>
        /// 200 OK - Success.
        /// <response code="400"> Bad Request </response> 
        /// <response code="500"> Internal Server Error </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="403"> Forbidden </response>
        /// <response code="404"> Not Found </response>
        [Permissions("Admin,EndUser")]
        [HttpPut("items")]
        public async Task<IActionResult> UpdateCartItem([FromForm]List<CartItemRQ> cartItemRQ)
        {
            var updatedCartItem = await _cartService.UpdateCartItemAsync(cartItemRQ);
            if (updatedCartItem == null)
            {
                return NotFound(ResponseHandler.HandleError<string>("NotFound", "no items present"));
            }

            return Ok(ResponseHandler.HandleSuccess("Successfully updated the cart", updatedCartItem));
        }

        /// <summary>
        /// Initiates the checkout process for the user's cart.
        /// </summary>
        /// 200 OK - Success.
        /// <response code="400"> Bad Request </response> 
        /// <response code="500"> Internal Server Error </response>
        /// <response code="401"> Unauthorized </response>
        /// <response code="403"> Forbidden </response>
        /// <response code="404"> Not Found </response>
        [Permissions("EndUser")]
        [HttpGet("/checkout")]
        public async Task<IActionResult> Checkout()
        {
            var updatedCartItem = await _cartService.CheckOutService();
            if (updatedCartItem == null)
            {
                return NotFound(ResponseHandler.HandleError<string>("NotFound", "no item found"));
            }
            return Ok(ResponseHandler.HandleSuccess("Successfully checked out of cart", updatedCartItem));
        }
        /// <summary>
        /// Removes an item from the cart by course ID.
        /// </summary>
        /// <param name="courseid">The ID of the course to remove from the cart.</param>
        /// <returns>
        /// An IActionResult indicating the result of the operation. 
        /// Returns a 404 status code if the item is not found, or a 200 status code with a success message if the item is successfully removed.
        /// </returns>
        /// <response code="200">Item successfully removed from the cart.</response>
        /// <response code="404">No item found with the specified course ID.</response>
        [Permissions("EndUser")]
        [HttpPut("remove/{courseid}")]
        public async Task<IActionResult> RemoveCartItem(int courseid)
        {
            var removedEntry = await _cartService.RemoveCartItemServiceAsync(courseid);
            if (removedEntry == null)
            {
                return NotFound(ResponseHandler.HandleError<string>("NotFound", "no item found"));
            }
            return Ok(ResponseHandler.HandleSuccess("Successfully removed out of cart", removedEntry));
        }


    }
}
