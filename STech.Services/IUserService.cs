using STech.Data.Models;
using STech.Data.ViewModels;

namespace STech.Services
{
    public interface IUserService
    {
        Task<int> GetTotalUsers();
        Task<IEnumerable<User>> GetTopUsers(int numToTake);

        Task<User?> GetUser(LoginVM login);
        Task<User?> GetUserById(string id);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetEmployeeUser(string userId);

        Task<bool> IsExist(string username);
        Task<bool> IsEmailExist(string email);
        Task<bool> IsEmailExist(string userId, string email);
        Task<bool> CreateUser(RegisterVM register);
        Task<bool> CreateUser(ExternalRegisterVM register);
        Task<bool> UpdateUser(User user);

        Task<UserAddress?> GetUserMainAddress(string id);
        Task<IEnumerable<UserAddress>> GetUserAddress(string userId);
        Task<UserAddress?> GetUserAddress(string userId, int addressId);
        Task<bool> CreateUserAddress(UserAddress address);
        Task<bool> UpdateUserAddress(UserAddress address);
        Task<bool> SetDefaultAddress(string userId, int id);
        Task<bool> DeleteUserAddress(string userId, int id);
    }
}
