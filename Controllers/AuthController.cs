﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog_API.Data;
using Blog_API.Dtos;
using Blog_API.Helpers;
using Blog_API.Mappers;
using Blog_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _repo;

        public AuthController(IUserRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterDTO userRegisterDTO)
        {
            if (ModelState.ErrorCount > 0)
                return BadRequest(ModelState);

            if (await _repo.UserExists(userRegisterDTO.email.Trim()))
                return BadRequest("Email is already Taken");

            var user = await _repo.Register(userRegisterDTO);
            if(user != null)
            {
                await _repo.AddRole(user.UID, "User");
                return Ok(user);
            }
            else
            {
                return BadRequest("Problrem ocour");
            }
            
        }
       

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDTO userLoginDTO)
        {
            var user = await _repo.Login(userLoginDTO);
            if (user == null)
                return BadRequest("Email or Password incorrect");

            DateTime expiration = DateTime.Now.AddDays(1);
            var token = JWTHelper.createLoginToken(user, expiration);

            return Ok(new { token = token,expiration = expiration, uemail = user.Uemail,uid = user.UID });
           
        }


        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    var clients = await _repo.GetAll();
        //    return Ok(clients);
        //}
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    var posts = await _repo.GetByField(x => x.CID == id);
        //    return Ok(posts);
        //}
        //public IActionResult Index()
        //{
        //    return View();
        //}

       
    }
}