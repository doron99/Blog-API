using Blog_API.Dtos;
using Blog_API.Helpers;
using Blog_API.Mappers;
using Blog_API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _db;

        public UserRepository(DataContext db)
        {
            _db = db;
        }

        
        //functions from irepository
        public void Add(User item)
        {
            _db.Add(item);
        }

       

        public void Delete(User item)
        {
            throw new NotImplementedException();
        }

        public IQueryable<User> GetAll()
        {
            return _db.Users.AsQueryable();
        }

        public async Task<User> Login(UserLoginDTO userLoginDTO)
        {
            var userFromRepo = await GetAll().Where(x => x.Uemail.Equals(userLoginDTO.email)).FirstOrDefaultAsync();
            if (userFromRepo == null)
                return null;

            if (!Helper.VerifyPasswordHash(userLoginDTO.password, userFromRepo.PasswordHash, userFromRepo.PasswordSalt))
                return null;

            return userFromRepo;
        }

        public async Task<bool> Register(UserRegisterDTO userRegisterDTO)
        {
            var userToCreate = Mapper.UserRegisterDTOToUser(userRegisterDTO);
            Helper.CreatePasswordHash(userRegisterDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);
            userToCreate.PasswordHash = passwordHash;
            userToCreate.PasswordSalt = passwordSalt;
            Add(userToCreate);
            if (await SaveAll())
                return true;
            else
                return false;
        }

        public async Task<bool> SaveAll()
        {
            return await _db.SaveChangesAsync() > 0 ? true : false;
        }

        public void Update(User item)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UserExists(string email)
        {
            return await GetAll().AnyAsync(x => x.Uemail.Equals(email)) ? true : false;
        }
    }
}
