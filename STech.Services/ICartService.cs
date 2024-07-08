using STech.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Services
{
    public interface ICartService
    {
        Task<UserCart?> GetUserCartItem(string userId, string productId);
        Task<IEnumerable<UserCart>> GetUserCart(string userId);
        Task<bool> AddToCart(UserCart cart);
        Task<bool> RemoveFromCart(UserCart cart);
        Task<bool> RemoveUserCart(string userId);
        Task<bool> RemoveListCart(IEnumerable<UserCart> cart);
        Task<bool> UpdateQuantity(UserCart cart, int qty);
    }
}
