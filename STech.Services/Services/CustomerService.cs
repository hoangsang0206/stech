using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services.Utils;

namespace STech.Services.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly StechDbContext _context;

        public CustomerService(StechDbContext context) {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> SearchCustomers(string phone)
        {
            return await _context.Customers
                .Where(c => c.Phone.Contains(phone))
                .ToListAsync();
        }

        public async Task<Customer?> GetCustomerById(string id)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == id);
        }

        public async Task<bool> CreateCustomer(Customer customer)
        {
            customer.CustomerId = DateTime.Now.ToString("yyyyMMdd") + UserUtils.GenerateRandomString(8).ToUpper();

            _context.Customers.Add(customer);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
