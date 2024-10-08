﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Services.Services
{
    public class UserService : IUserService
    {
        private readonly StechDbContext _context;
        public UserService(StechDbContext context) => _context = context;

        #region User

        public async Task<User?> GetUser(LoginVM login)
        {
            User? user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == login.UserName);

            if(user != null && user.PasswordHash == login.Password.HashPasswordMD5(user.RandomKey))
            {
                return user;
            }

            return null;
        }

        public async Task<User?> GetUserById(string id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> IsExist(string username)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            return user != null;
        }

        public async Task<bool> IsEmailExist(string email)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user != null;
        }

        public async Task<bool> IsEmailExist(string userId, string email)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.UserId != userId);
            return user != null;
        }

        public async Task<bool> CreateUser(RegisterVM register)
        {
            if(register.RegPassword != register.ConfirmPassword) return false;

            string randomKey = UserUtils.GenerateRandomString(20);

            Role? role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == "user");

            if(role == null || role.RoleId == null) return false;

            User user = new User()
            {
                UserId = UserUtils.GenerateRandomId(40),
                Username = register.RegUserName,
                PasswordHash = register.RegPassword.HashPasswordMD5(randomKey),
                Email = register.Email,
                RandomKey = randomKey,
                IsActive = true,
                RoleId = role.RoleId,
            };

            await _context.Users.AddAsync(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CreateUser(ExternalRegisterVM register)
        {
            string randomKey = UserUtils.GenerateRandomString(20);

            Role? role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == "user");

            if (role == null || role.RoleId == null)
            {
                return false;
            }

            User user = new User()
            {
                UserId = register.UserId,
                Username = register.UserId ?? UserUtils.GenerateRandomId(30),
                PasswordHash = UserUtils.GenerateRandomString(20).HashPasswordMD5(randomKey),
                Email = register.Email,
                EmailConfirmed = register.EmailConfirmed,
                RandomKey = randomKey,
                FullName = register.FullName,
                Avatar = register.Avatar,
                IsActive = true,
                RoleId = role.RoleId,
                AuthenticationProvider = register.AuthenticationProvider
            };

            await _context.Users.AddAsync(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateUser(User user)
        {
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        #endregion


        #region UserAddresses

        public async Task<UserAddress?> GetUserMainAddress(string id)
        {
            return await _context.UserAddresses
                .FirstOrDefaultAsync(u => u.UserId == id && u.IsDefault != null && u.IsDefault.Value);
        }

        public async Task<IEnumerable<UserAddress>> GetUserAddress(string userId)
        {
            return await _context.UserAddresses.Where(u => u.UserId == userId).OrderBy(d => !d.IsDefault).ToListAsync();
        }

        public async Task<UserAddress?> GetUserAddress(string userId, int addressId)
        {
            return await _context.UserAddresses
                .FirstOrDefaultAsync(u => u.UserId == userId && u.Id == addressId);
        }

        public async Task<bool> CreateUserAddress(UserAddress address)
        {
            _context.UserAddresses.Add(address);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateUserAddress(UserAddress address)
        {
            _context.UserAddresses.Update(address);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SetDefaultAddress(string userId, int id)
        {
            UserAddress? address = await _context.UserAddresses
                .FirstOrDefaultAsync(u => u.UserId == userId && u.Id == id);
            UserAddress? mainAddress = await GetUserMainAddress(userId);

            if (address != null)
            {
                if(mainAddress != null)
                {
                    mainAddress.IsDefault = false;
                    _context.UserAddresses.Update(mainAddress);
                }

                address.IsDefault = true;

                _context.UserAddresses.Update(address);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<bool> DeleteUserAddress(string userId, int id)
        {
            UserAddress? address = await _context.UserAddresses.FirstOrDefaultAsync(u => u.UserId == userId && u.Id == id);
            if(address == null) return false;
            if (Convert.ToBoolean(address.IsDefault)) return false;

            _context.UserAddresses.Remove(address);
            return await _context.SaveChangesAsync() > 0;
        }

        #endregion
    }
}
