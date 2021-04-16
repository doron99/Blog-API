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

        #region roles
        public async Task<bool> AddRole(int id,string role)
        {
            _db.Roles.Add(new Role { UID = id, RoleName = role });
            if (await _db.SaveChangesAsync() > 0)
                return true;
            else
                return false;
        }
        public async Task<bool> DeleteRole(int id, string role)
        {
            var roleFromDB = await _db.Roles.FirstOrDefaultAsync(x => x.UID == id && x.RoleName.Equals(role));
            if (roleFromDB == null)
                return false;

            _db.Roles.Remove(roleFromDB);
            if (await _db.SaveChangesAsync() > 0)
                return true;
            else
                return false;
        }
        #endregion


        #region authentication
        public async Task<User> Login(UserLoginDTO userLoginDTO)
        {
            var userFromRepo = await GetAll().Where(x => x.Uemail.Equals(userLoginDTO.email)).Include(x => x.Roles).FirstOrDefaultAsync();
            if (userFromRepo == null)
                return null;

            if (!Helper.VerifyPasswordHash(userLoginDTO.password, userFromRepo.PasswordHash, userFromRepo.PasswordSalt))
                return null;

            return userFromRepo;
        }

        public async Task<User> Register(UserRegisterDTO userRegisterDTO)
        {
            var userToCreate = Mapper.UserRegisterDTOToUser(userRegisterDTO);
            Helper.CreatePasswordHash(userRegisterDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);
            userToCreate.PasswordHash = passwordHash;
            userToCreate.PasswordSalt = passwordSalt;
            Add(userToCreate);
            if (await SaveAll())
                return userToCreate;
            else
                return null;
        }

        public async Task<bool> UserExists(string email)
        {
            return await GetAll().AnyAsync(x => x.Uemail.Equals(email)) ? true : false;
        }
        #endregion
        

        #region functions from irepository
        public async Task<bool> SaveAll()
        {
            return await _db.SaveChangesAsync() > 0 ? true : false;
        }

        public void Update(User item)
        {
            _db.Entry(item).State = EntityState.Modified;
        }
        public void Add(User item)
        {
            _db.Add(item);
        }
        public void Delete(User item)
        {
            _db.Remove(item);
        }

        public IQueryable<User> GetAll()
        {
            return _db.Users.AsQueryable();
        }
        #endregion



    }
}
