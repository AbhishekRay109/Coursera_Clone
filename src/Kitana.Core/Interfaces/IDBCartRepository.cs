using Scaf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitana.Core.Interfaces
{
    public interface IDBCartRepository
    {
        public Task<IEnumerable<CartItem>> GetCartItemsAsync(int userId);
        public Task<CartItem> AddCartItemAsync(CartItem cartItem);
        public Task<List<CartItem>> GetCartItemByIdAsync(int itemId);

        public Task<ShoppingCart> GetShoppingCartByUserIdAsync(int userId);

        public Task<SFTransaction> CheckoutRespositoryAsync(int userId);
        public Task<CartItem> UpdateQuantityAsync(int cartItemId, int quantity);
        public Task<CartItem> RemoveCartItemRepoAsync(int courseId, int userId);
        public Task<CartItem> UpdateCartItemAsync(CartItem cartItem);
        public Task<ShoppingCart> ShoppingCartAsync(int userId);


    }
}
