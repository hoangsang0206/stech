using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Services.Services
{
    public class CartService : ICartService
    {
        private readonly StechDbContext _context;
        public CartService(StechDbContext context) => _context = context;

        public async Task<bool> AddToCart(UserCart cart)
        {
            if(cart == null || cart.UserId == null || cart.ProductId == null || cart.Quantity <= 0) { 
                return false; 
            }

            await _context.UserCarts.AddAsync(cart);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<UserCart?> GetUserCartItem(string userId, string productId)
        {
            if(userId == null || productId == null)
            {
                return new UserCart();
            }

            return await _context.UserCarts.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
        }

        public async Task<IEnumerable<UserCart>> GetUserCart(string userId)
        {
            if(userId == null)
            {
                return new List<UserCart>();
            }

            return await _context.UserCarts
                .Where(c => c.UserId == userId)
                .Select(c => new UserCart
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    Product = new Product
                    {
                        ProductId = c.ProductId,
                        ProductName = c.Product.ProductName,
                        OriginalPrice = c.Product.OriginalPrice,
                        Price = c.Product.Price,
                        ProductImages = c.Product.ProductImages.OrderBy(pp => pp.Id).Take(1).ToList(),
                    },
                    Quantity = c.Quantity,
                })
                .ToListAsync();
        }

        public async Task<bool> RemoveFromCart(UserCart cart)
        {
            if (cart == null)
            {
                return false;
            }

            _context.UserCarts.Remove(cart);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveUserCart(string userId)
        {
            if(userId == null)
            {
                return false;
            }

            IEnumerable<UserCart> cart = await _context.UserCarts.Where(c => c.UserId == userId).ToListAsync();

            if(cart.Count() <= 0)
            {
                return false;
            }

            _context.UserCarts.RemoveRange(cart);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveListCart(IEnumerable<UserCart> cart)
        {
            if(cart.Count() <= 0)
            {
                return false;
            }

            _context.UserCarts.RemoveRange(cart);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateQuantity(UserCart cart, int qty)
        {
            if (cart == null)
            {
                return false;
            }

            UserCart? dbCart = await _context.UserCarts.FirstOrDefaultAsync(c => c.Id == cart.Id);
            if(dbCart == null)
            {
                return false;
            }

            dbCart.Quantity = qty;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
