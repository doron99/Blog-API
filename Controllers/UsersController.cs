using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blog_API.Classes;
using Blog_API.Data;
using Blog_API.Dtos;
using Blog_API.Helpers;
using Blog_API.Mappers;
using Blog_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public UsersController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Users([FromQuery] PaginationFilter filter)
        {

            var query = _userRepo.GetAll();//.Include(x => x.Roles);

            var validFilter = new PaginationFilter(filter.CurrPage, filter.ItemsPerPage);
            var rowCount = await query.CountAsync();
            var results = await query
                .Skip((validFilter.CurrPage - 1) * validFilter.ItemsPerPage)
                .Take(validFilter.ItemsPerPage).ToListAsync();

            return Ok(PaginationHelper.CreatePagedReponse<User>(results, validFilter, rowCount));

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Users(int id)
        {
            var userFromDB = await _userRepo.GetAll().AsQueryable()
                .Where(x => x.UID == id)
                .Include(x => x.Roles)
                .FirstOrDefaultAsync();

            return Ok(userFromDB);
        }
        [Authorize(Roles = "Administrator,User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Users(int id, UserUpdateDTO userUpdateDTO)
        {

            if (userUpdateDTO == null)
                return BadRequest();

            var userFromDB = await _userRepo.GetAll().FirstOrDefaultAsync(x => x.UID == id);

            //validate new email does not exists in system
            if(!userFromDB.Uemail.Trim().Equals(userUpdateDTO.Uemail.Trim())){
                if (await _userRepo.UserExists(userUpdateDTO.Uemail.Trim()))
                    return BadRequest("email already taken");
            }

            int userid = Convert.ToInt32(User.Claims.Where(x => x.Type == "UserId").FirstOrDefault()?.Value);
            var roles = User.Claims.Where(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").ToList();
            var isAdmin = roles.FirstOrDefault(role => role.Value.Contains("Administrator")) != null ? true : false;
            var isUser = roles.FirstOrDefault(role => role.Value.Contains("User")) != null ? true : false;

            //check if not admin
            if (userid != id && !isAdmin)
                return Unauthorized("you have no permission for other accounts");


            if (userFromDB == null)
                return NotFound();

            //the function setPostUpdateFields is in mapper class.
            userFromDB.setUserUpdateFields(userUpdateDTO);

            _userRepo.Update(userFromDB);
            try
            {
                if (await _userRepo.SaveAll())
                    return NoContent();
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
            return BadRequest();

        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> Users(int id, [FromBody] JsonPatchDocument<User> patchEntity)
        {

            if (patchEntity == null)
                return BadRequest();

            var userFromDB = await _userRepo.GetAll().FirstOrDefaultAsync(x => x.UID == id);
            int userid = Convert.ToInt32(User.Claims.Where(x => x.Type == "UserId").FirstOrDefault()?.Value);

            if (userFromDB == null)
                return NotFound();


            patchEntity.ApplyTo(userFromDB, ModelState);

            if (await _userRepo.SaveAll())
                return Ok(userFromDB);
            else
                return BadRequest();
            //    [
            //    {
            //        "value": "Friday the 13th",
            //        "path": "/Title",
            //        "op": "replace"
            //    },
            //    {
            //        "value": "2020-04-01",
            //        "path": "/releaseDate",
            //        "op": "replace"
            //    }    
            //    ]
        }
        [HttpGet("{id}/role/{role}")]
        public async Task<IActionResult> AddRole(int id,string role)
        {
            var flag = await _userRepo.AddRole(id, role);
            if (flag)
                return Ok();
            else
                return BadRequest();
        }
        [HttpDelete("{id}/role/{role}")]
        public async Task<IActionResult> DeleteRole(int id, string role)
        {
            var flag = await _userRepo.DeleteRole(id, role);
            if (flag)
                return Ok();
            else
                return BadRequest();
        }
        [HttpPost("{id}/ChangePassword")]
        public async Task<IActionResult> ChangePassword(int id, ChangePasswordDTO changePasswordDTO)
        {
            if (ModelState.ErrorCount > 0)
                return BadRequest(ModelState);

            var userFromDB = await _userRepo.GetAll().FirstOrDefaultAsync(x => x.UID == id);
            if (userFromDB == null)
                return NotFound();

            if(changePasswordDTO.UID != userFromDB.UID)
                return NotFound();

            int userid = Convert.ToInt32(User.Claims.Where(x => x.Type == "UserId").FirstOrDefault()?.Value);
            var roles = User.Claims.Where(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").ToList();
            var isAdmin = roles.FirstOrDefault(role => role.Value.Contains("Administrator")) != null ? true : false;


            //check if not admin
            if (userid != id && !isAdmin)
                return Unauthorized("you have no permission for other accounts");


           

            Helper.CreatePasswordHash(changePasswordDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);
            userFromDB.PasswordHash = passwordHash;
            userFromDB.PasswordSalt = passwordSalt;

            _userRepo.Update(userFromDB);
            if (await _userRepo.SaveAll())
                return Ok();
            else
                return BadRequest();


        }


    }
}