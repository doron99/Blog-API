using Blog_API.Dtos;
using Blog_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_API.Data
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> Register(UserRegisterDTO userRegisterDTO);
        Task<User> Login(UserLoginDTO userLoginDTO);
        Task<bool> UserExists(string email);
        Task<bool> AddRole(int id, string role);
        Task<bool> DeleteRole(int id, string role);
    }
}
