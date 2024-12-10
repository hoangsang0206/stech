using STech.Data.Models;
using STech.Data.ViewModels;

namespace STech.Services
{
    public interface IEmployeeService
    {
        Task<Employee?> GetEmployeeByUserId(string userId);
        Task<Employee?> GetEmployeeById(string id);
        Task<PagedList<Employee>> GetEmployees(int page, int pageSize);
        Task<PagedList<Employee>> SearchEmployees(string? phone, string? email, string? employeeName, int page, int pageSize);
    }
}
