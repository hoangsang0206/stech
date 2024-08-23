using Microsoft.EntityFrameworkCore;
using STech.Data.Models;

namespace STech.Services.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly StechDbContext _context;

        public EmployeeService(StechDbContext context)
        {
            _context = context;
        }

        public async Task<Employee?> GetEmployeeByUserId(string userId)
        {
            User? user = await _context.Users
                .Where(u => u.UserId == userId)
                .Include(u => u.Employee)
                .FirstOrDefaultAsync();

            return user?.Employee;
        }
    }
}
