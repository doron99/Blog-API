using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blog_API.Data;
using Blog_API.Dtos;
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
    public class PostsController : ControllerBase
    {
        private readonly IPostRepository _postRepo;
        private readonly ICommentRepository _commentRepo;

        public PostsController(IPostRepository postRepo, ICommentRepository commentRepo)
        {
            _postRepo = postRepo;
            _commentRepo = commentRepo;
        }
        [HttpGet]
        public async Task<IActionResult> Posts()
        {
            var list = await _postRepo.GetAll().AsQueryable().Include(x => x.Author).Include(c => c.Comments).AsNoTracking().ToListAsync();
            return Ok(list);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Posts(int id)
        {
            var list = await _postRepo.GetAll().AsQueryable().Where(x => x.PostId == id).Include(x => x.Author).Include(c => c.Comments).FirstOrDefaultAsync();
            return Ok(list);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Posts(PostCreateDTO postCreateDTO)
        {
            var post = Mapper.PostCreateDTOToPost(postCreateDTO);
            _postRepo.Add(post);
            if (await _postRepo.SaveAll())
                return Created("/","");
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
            if (await _postRepo.SaveAll())
                return NoContent();
            else
                return BadRequest();

        }

        [HttpPost("/api/Comments")]
        [Authorize]
        public async Task<IActionResult> Comments(CommentCreateDTO commentCreateDTO)
        {
            int userid = Convert.ToInt32(User.Claims.Where(x => x.Type == "UserId").FirstOrDefault()?.Value);

            var comment = Mapper.CommentCreateDTOToComment(commentCreateDTO);
            comment.AuthorId = userid;
            _commentRepo.Add(comment);
            if (await _commentRepo.SaveAll())
                return Created("/", "");
            else
                return BadRequest();
        }

    }
}