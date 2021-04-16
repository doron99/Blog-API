using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blog_API.Classes;
using Blog_API.Data;
using Blog_API.Dtos;
using Blog_API.Helpers;
using Blog_API.Helpers;

using Blog_API.Mappers;
using Blog_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostRepository _postRepo;
        private readonly ICommentRepository _commentRepo;
        private readonly IFileRepository _fileRepo;

        public PostsController(
            IPostRepository postRepo, 
            ICommentRepository commentRepo,
            IFileRepository fileRepo)
        {
            _fileRepo = fileRepo;
            _postRepo = postRepo;
            _commentRepo = commentRepo;
        }
        
        [HttpGet]
        public async Task<IActionResult> Posts([FromQuery] PaginationFilter filter)
        {

            var query = _postRepo.GetAll()
                .WhereIf(!filter.mode.Equals("manager"), x => x.Public == true)
                .IncludeIf(!filter.mode.Equals("manager"), x => x.Include(a => a.Author));


            var validFilter = new PaginationFilter(filter.CurrPage, filter.ItemsPerPage);
            var rowCount = await query.CountAsync();
            var results = await query.Skip((validFilter.CurrPage - 1) * validFilter.ItemsPerPage).Take(validFilter.ItemsPerPage).ToListAsync();

            return Ok(PaginationHelper.CreatePagedReponse<Post>(results, validFilter, rowCount));

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Posts(int id)
        {
            var list = await _postRepo.GetAll().AsQueryable()
                .Where(x => x.PostId == id)
                .Include(x => x.Author)
                .FirstOrDefaultAsync();

            return Ok(list);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Posts(PostCreateDTO postCreateDTO)
        {
            
            int userid = Convert.ToInt32(User.Claims.Where(x => x.Type == "UserId").FirstOrDefault()?.Value);

            var post = Mapper.PostCreateDTOToPost(postCreateDTO);
            post.AuthorId = Convert.ToInt32(userid);
            _postRepo.Add(post);
            if (await _postRepo.SaveAll())
                //return Created("/","");
                return Ok(post);
            else
                return BadRequest();
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> Posts(int id, [FromBody] JsonPatchDocument<Post> patchEntity)
        {

            if (patchEntity == null)
                return BadRequest();

            var postFromDB = await _postRepo.GetAll().FirstOrDefaultAsync(x => x.PostId == id);
            int userid = Convert.ToInt32(User.Claims.Where(x => x.Type == "UserId").FirstOrDefault()?.Value);
            //check if is the same owner
            if(postFromDB.AuthorId != userid)
                return Unauthorized("your'e not authorized to update this post");
            
            if (postFromDB == null)
                return NotFound();

            patchEntity.ApplyTo(postFromDB, ModelState);

            if (await _postRepo.SaveAll())
                return Ok(postFromDB);
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
        [HttpPut("{id}")]
        public async Task<IActionResult> Posts(int id,PostUpdateDTO postUpdateDTO)
        {

            if (postUpdateDTO == null)
                return BadRequest();

            var postFromDB = await _postRepo.GetAll().FirstOrDefaultAsync(x => x.PostId == id);
            int userid = Convert.ToInt32(User.Claims.Where(x => x.Type == "UserId").FirstOrDefault()?.Value);
            //check if is the same owner
            if (postFromDB.AuthorId != userid)
                return Unauthorized("your'e not authorized to update this post");
            if (postFromDB == null)
                return NotFound();

            //the function setPostUpdateFields is in mapper class.
            postFromDB.setPostUpdateFields(postUpdateDTO);

            _postRepo.Update(postFromDB);
            try
            {
                if (await _postRepo.SaveAll())
                    return NoContent();
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                var x = ex.Message;
            }
            return BadRequest();

        }


        [HttpPost("/api/posts/{id}/upload")]
        [Authorize]
        public async Task<IActionResult> upload(int id, IFormFile image)
        {
            var postFromDB = await _postRepo.GetAll().Where(x => x.PostId == id).FirstOrDefaultAsync();
            if (postFromDB == null)
                return NotFound();

            int userid = Convert.ToInt32(User.Claims.Where(x => x.Type == "UserId").FirstOrDefault()?.Value);

            //check if is the same owner
            if (postFromDB.AuthorId != userid)
                return Unauthorized("your'e not authorized to upload in this post");

            if (!string.IsNullOrEmpty(postFromDB.CoverImagePath))
            {
                _fileRepo.deleteFileByName("posts", postFromDB.CoverImagePath);
            }

            if (image != null)
            {
                string prefix = Helper.getSaltString();
                string suffix = Path.GetExtension(image.FileName);

                var img = _fileRepo.uploadImg(image, "posts", prefix + "" + suffix, true, 800, false);

                if (!string.IsNullOrEmpty(img))
                {
                    postFromDB.CoverImagePath = img;
                    _postRepo.Update(postFromDB);
                    if (await _postRepo.SaveAll())
                    {
                        return Ok(img);
                    }
                }

            }
            return BadRequest();

        }

    }
}