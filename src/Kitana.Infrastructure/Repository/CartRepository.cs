using Kitana.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Scaf.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Transactions;
using ZstdSharp.Unsafe;

namespace Kitana.Infrastructure.Repository
{
    public class CartRepository : IDBCartRepository
    {
        private readonly SkillForgeDBContext _context;
        private readonly JsonSerializerOptions _jsonSerializerOptions;


        public CartRepository(SkillForgeDBContext context)
        {
            _context = context;
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                MaxDepth = 64 // Adjust depth limit if needed
            };

        }

        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(int userId)
        {
            var cart = await _context.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            if (cart == null)
            {
                // Handle scenario where cart is not found for the specified user
                return Enumerable.Empty<CartItem>(); // Return an empty enumerable
            }

            var cartItems = await _context.CartItems
                .Include(b => b.Course)
                .Where(item => item.CartId == cart.CartId)
                .ToListAsync();

            // Serialize cartItems to JSON with custom options
            var serializedCartItems = JsonSerializer.Serialize(cartItems, _jsonSerializerOptions);

            return cartItems;
        }

        public async Task<CartItem> AddCartItemAsync(CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
            var output = await _context.CartItems.Include(a => a.Cart).Include(b=>b.Course).FirstOrDefaultAsync(x => x.CartItemId == cartItem.CartItemId);
            return output;
        }
        public async Task<CartItem> UpdateQuantityAsync(int cartItemId, int quantity)
        {
            var cart = await _context.CartItems.FirstOrDefaultAsync(x => x.CartItemId == cartItemId);
            cart.Quantity += quantity;
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<CartItem> UpdateCartItemAsync(CartItem cartItem)
        {
            _context.Entry(cartItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            var output = await _context.CartItems.Include(a => a.Cart).Include(b => b.Course).FirstOrDefaultAsync(x => x.CartItemId == cartItem.CartItemId);
            return output;
        }

        public async Task<CartItem> RemoveCartItemRepoAsync(int courseId, int userId)
        {
            var shoppingCart = await _context.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == userId);
            if(shoppingCart == null)
            {
                throw new ArgumentException("Cart Empty");
            }
            var output = await _context.CartItems.Include(a => a.Cart).Include(b => b.Course)
                .FirstOrDefaultAsync(x => x.CourseId == courseId && x.CartId == shoppingCart.CartId);
            if (output == null)
            {
                throw new ArgumentException("item not found");
            }
            _context.Remove(output);
            await _context.SaveChangesAsync();
            return output;
        }

        public async Task<SFTransaction> CheckoutRespositoryAsync(int userId)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            try
            {
                var cart = await _context.ShoppingCarts.FirstOrDefaultAsync(c => c.UserId == userId);
                var items = await _context.CartItems.Include(x => x.Course).Where(it => it.Cart.UserId == userId).ToListAsync();
                decimal? bill = 0;
                if (items == null)
                {
                    throw new ArgumentException("No item in the cart");
                }
                string courseIDsCsv = "";
                foreach (var item in items)
                {
                    var courseDetail = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == item.CourseId);
                    if (courseDetail.Price != null)
                    {
                        bill += ((courseDetail.Price * (100 - item.Discount) / 100)) * item.Quantity;
                    }
                    var courseIdList = items.Select(item => item.CourseId).ToList();
                    courseIDsCsv = string.Join(",", courseIdList);
                    UserCourse userCourse = new()
                    {
                        UserId = userId,
                        CourseId = item.CourseId,
                        CompletedPercent = 0,
                        AvailableTill = DateTime.Now.AddDays(courseDetail.ActiveTime)
                    };
                    _context.Add(userCourse);
                    await _context.SaveChangesAsync();
                }

                SFTransaction trans = new()
                {
                    CourseIds = courseIDsCsv,
                    TotalAmount = bill,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    TransactionStatus = "Success"
                };
                _context.Add(trans);
                await _context.SaveChangesAsync();
                var transact = await _context.Transactions.FirstOrDefaultAsync(x => x.TransactionId == trans.TransactionId);
                _context.Remove(cart);
                await _context.SaveChangesAsync();

                scope.Complete(); // Commit the transaction
                return transact;
            }
            catch (Exception)
            {
                scope.Dispose(); // Rollback the transaction
                throw;
            }
        }

        public async Task<List<CartItem>> GetCartItemByIdAsync(int itemId)
        {
            return await _context.CartItems.Include(a => a.Cart).Include(b => b.Course)
                .Where(x => x.CartId == itemId).ToListAsync();
        }
        public async Task<ShoppingCart> GetShoppingCartByUserIdAsync(int userId)
        {
            var cart =await  _context.ShoppingCarts.Include(x => x.CartItems).FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;
        }
        public async Task<ShoppingCart> ShoppingCartAsync(int userId)
        {
            var shopping = new ShoppingCart()
            {
                UserId = userId,
                CreatedAt = DateTime.Now
            };
            _context.ShoppingCarts.Add(shopping);
            await _context.SaveChangesAsync();
            return shopping;
        }

    }
}
