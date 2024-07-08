using STech.Data.Models;
using STech.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Services
{
    public interface IUserService
    {
        Task<User?> GetUser(LoginVM login);
        Task<User?> GetUserById(string id);
        Task<bool> IsExist(string username);
        Task<bool> IsEmailExist(string email);
        Task<bool> CreateUser(RegisterVM register);
    }
}
