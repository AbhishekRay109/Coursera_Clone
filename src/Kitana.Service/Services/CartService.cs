using Kitana.Core.Interfaces;
using Kitana.Service.Model;
using Microsoft.AspNetCore.Http;
using Scaf.Models;

namespace Kitana.Service.Services
{
    public class CartService
    {
        private readonly IDBCartRepository _cartRepository;
        private readonly ISession _session;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CartService(IDBCartRepository cartRepository, IHttpContextAccessor accessor)
        {
            httpContextAccessor = accessor;
            _session = httpContextAccessor.HttpContext.Session;
            _cartRepository = cartRepository;
        }

        public async Task<IEnumerable<CartItemRS>> GetCartItemsAsync()
        {           var userId = Convert.ToInt32(_session.GetString("UserId"));
            var cartItems = await _cartRepository.GetCartItemsAsync(userId);
            return cartItems.Select(MapToCartItemRS);
        }

        public async Task<CartItemRS> AddCartItemAsync(CartItemRQ cartItemRQ)
        {
            var userId = Convert.ToInt32(_session.GetString("UserId"));
            var cartItem = MapToCartItem(cartItemRQ);
            int cartid;
            try
            {
                var carts = (await _cartRepository.GetShoppingCartByUserIdAsync(userId));
                cartid = carts.CartId;
                foreach (var item1 in carts.CartItems)
                {
                    if(item1.CourseId == cartItemRQ.CourseId)
                    {
                        var cart = item1;
                        var cartUpdate = await _cartRepository.UpdateQuantityAsync(item1.CartItemId, 1);
                        return MapToCartItemRS(cartUpdate);
                    }
                }
            }
            catch
            {
                cartid = (await _cartRepository.ShoppingCartAsync(userId)).CartId;
            }
            cartItem.CartId = cartid;
            var addedCartItem = await _cartRepository.AddCartItemAsync(cartItem);
            return MapToCartItemRS(addedCartItem);
        }

        public async Task<List<CartItemRS>> UpdateCartItemAsync(List<CartItemRQ> cartItemRQ)
        {
            var userId = Convert.ToInt32(_session.GetString("UserId"));
            var cartId = (await _cartRepository.GetShoppingCartByUserIdAsync(userId))?.CartId;

            if (cartId == null)
            {
                return null;
            }
            var existingCartItems = await _cartRepository.GetCartItemByIdAsync(cartId.Value);
            var updatedCartItemResponses = new List<CartItemRS>();
            foreach (var rqItem in cartItemRQ)
            {
                var existingItem = existingCartItems.FirstOrDefault(item => item.CourseId == rqItem.CourseId);

                if (existingItem != null)
                {
                    // Update the existing cart item
                    MapCartItemRQToCartItem(rqItem, existingItem);
                    await _cartRepository.UpdateCartItemAsync(existingItem);
                }
                else
                {
                    // Add new cart item if it doesn't exist in the cart
                    var cartItemToAdd = MapToCartItem(rqItem);
                    cartItemToAdd.CartId = cartId.Value;
                    await _cartRepository.AddCartItemAsync(cartItemToAdd);
                }
            }

            // Return a response if needed (e.g., after processing all items)
            var updatedCartItems = await _cartRepository.GetCartItemByIdAsync(cartId.Value);
            foreach (var item in updatedCartItems)
            {
                updatedCartItemResponses.Add(MapToCartItemRS(item));
            }

            // For simplicity, assuming you return a list of updated cart item responses
            return updatedCartItemResponses; // Example; adjust as needed
        }


        public async Task<SFTransaction> CheckOutService()
        {
            var userId = Convert.ToInt32(_session.GetString("UserId"));
            var transaction = await _cartRepository.CheckoutRespositoryAsync(userId);
            return transaction;
        }
        public async Task<CartItemRS> RemoveCartItemServiceAsync(int courseId)
        {
            var userId = Convert.ToInt32(_session.GetString("UserId"));
            var removedData = await _cartRepository.RemoveCartItemRepoAsync(courseId,userId);
            return MapToCartItemRS(removedData);
        }

        private CartItemRS MapToCartItemRS(CartItem cartItem)
        {
            if (cartItem == null)
            {
                return null;
            }

            return new CartItemRS
            {
                CartItemId = cartItem.CartItemId,
                CourseName = cartItem.Course.Title,
                Quantity = cartItem.Quantity
            };
        }

        private CartItem MapToCartItem(CartItemRQ cartItemRQ)
        {
            return new CartItem
            {
                Discount = cartItemRQ.Discount,
                CourseId = cartItemRQ.CourseId,
                Quantity = 1
            };
        }

        private void MapCartItemRQToCartItem(CartItemRQ cartItemRQ, CartItem cartItem)
        {
            cartItem.CourseId = cartItemRQ.CourseId;
            cartItem.Quantity = 1;
        }
    }
}
