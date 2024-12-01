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
        
        public async Task<PagedList<Customer>> GetCustomers(int page, int itemsPerPage, string? filterBy, string? sortBy)
        {
            IQueryable<Customer> customers = _context.Customers;

            return await customers.ToPagedListAsync(page, itemsPerPage);
        }
        
        public async Task<Customer?> GetCustomerById(string id)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == id);
        }

        public async Task<bool> CreateCustomer(Customer customer)
        {
            customer.CustomerId = DateTime.Now.ToString("yyyyMMdd") + UserUtils.GenerateRandomString(8).ToUpper();
            customer.MemberSince = DateTime.Now;

            _context.Customers.Add(customer);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateCustomer(Customer customer)
        {
            _context.Customers.Update(customer);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCustomer(string id)
        {
            Customer? customer = await GetCustomerById(id);

            if (customer == null)
            {
                return false;
            }

            _context.Customers.Remove(customer);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
