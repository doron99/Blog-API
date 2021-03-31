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
        [Route("img")]
        public async Task<IActionResult> img(string path)
        {
            Byte[] b;
            try
            {
                b = await System.IO.File.ReadAllBytesAsync(path);
            }catch(Exception ex)
            {
                b = null;
            }
            
            return File(b, "image/jpeg");

            
        }
        [HttpGet]
        public async Task<IActionResult> Posts([FromQuery] PaginationFilter filter)
        {
            var list = await _postRepo.GetAll().AsQueryable().Include(x => x.Author)
                .ToListAsync();
            var query =  _postRepo.GetAll().Include(x => x.Author);

            var skip = (currPage - 1) * itemsPerPage;
            var take = itemsPerPage;

            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var rowCount = await query.CountAsync();
            var results = await query.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
               .Take(validFilter.PageSize).ToListAsync();

            return Ok(new PagedResponse<List<Post>>(results, validFilter.PageNumber, validFilter.PageSize));

            //return Ok(list);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Posts(int id)
        {
            var list = await _postRepo.GetAll().AsQueryable().Where(x => x.PostId == id).Include(x => x.Author).Include(s => s.Comments).FirstOrDefaultAsync();

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

        [HttpPost("/api/posts/{id}/comments")]
        [Authorize]
        public async Task<IActionResult> Comments(int id,CommentCreateDTO commentCreateDTO)
        {
            if (id != commentCreateDTO.PostId)
                return BadRequest();
            int userid = Convert.ToInt32(User.Claims.Where(x => x.Type == "UserId").FirstOrDefault()?.Value);

            var comment = Mapper.CommentCreateDTOToComment(commentCreateDTO);
            comment.AuthorId = userid;
            _commentRepo.Add(comment);
            try
            {
                if (await _commentRepo.SaveAll())
                {
                    var comments = await _commentRepo.GetAll()
                        .Where(c => c.PostId == id)
                        .Include(x => x.Author).ToListAsync();
                    return Ok(comments);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception ex)
            {
                var x = 1;
                return BadRequest();
            }  
        }
        [HttpGet("/api/posts/{id}/comments")]
       
        public async Task<IActionResult> Comments(int id)
        {
            if(await _postRepo.IsPostExists(id))
            {
                var comments = await _commentRepo.GetAll()
                    .Where(c => c.PostId == id)
                    .Include(x => x.Author).ToListAsync();
            return Ok(comments);
            }

            return BadRequest();
                



        }
        [HttpPost("/api/posts/{id}/upload")]
        [Authorize]
        public async Task<IActionResult> upload(int id, IFormFile image)
        {
            var post = await _postRepo.GetAll().Where(x => x.PostId == id).FirstOrDefaultAsync();
            if (post == null)
                return NotFound();

            if (!string.IsNullOrEmpty(post.CoverImagePath))
            {
                string[] fn = post.CoverImagePath.Split(".");
                string filename = fn[0].Replace("_sm", "");
                string extension = fn[1];

                _fileRepo.deleteFileByName("posts", post.CoverImagePath);

            }

            if (image != null)
            {
                string ImageName = Helper.getSaltString() + Path.GetExtension(image.FileName);
                string prefix = Helper.getSaltString();
                string suffix = Path.GetExtension(image.FileName);
                string[] fn = ImageName.Split(".");

                var img = _fileRepo.uploadImg(image, "posts", prefix + "" + suffix, true, 800, false);

                if (!string.IsNullOrEmpty(img))
                {
                    post.CoverImagePath = img;
                    _postRepo.Update(post);
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