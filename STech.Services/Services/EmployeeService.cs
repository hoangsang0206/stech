using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services.Utils;

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

        public async Task<Employee?> GetEmployeeById(string id)
        {
            return await _context.Employees
                .Where(e => e.EmployeeId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<PagedList<Employee>> GetEmployees(int page, int pageSize)
        {
            return await _context.Employees
                .OrderBy(e => e.EmployeeName)
                .ToPagedListAsync(page, pageSize);
        }

        public async Task<PagedList<Employee>> SearchEmployees(string? phone, string? email, string? employeeName, int page, int pageSize)
        {
            IQueryable<Employee> query = _context.Employees;
            if (!string.IsNullOrWhiteSpace(phone))
            {
                query = query.Where(e => e.Phone.Contains(phone));
            }
            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(e => e.Email.Contains(email));
            }
            if (!string.IsNullOrWhiteSpace(employeeName))
            {
                query = query.Where(e => e.EmployeeName.Contains(employeeName));
            }

            return await query.OrderBy(e => e.EmployeeName).ToPagedListAsync(page, pageSize);
        }
    }
}
