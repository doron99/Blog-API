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
    public class CommentsController : ControllerBase
    {
        private readonly IPostRepository _postRepo;
        private readonly ICommentRepository _commentRepo;
        private readonly IFileRepository _fileRepo;

        public CommentsController(
            IPostRepository postRepo, 
            ICommentRepository commentRepo,
            IFileRepository fileRepo)
        {
            _fileRepo = fileRepo;
            _postRepo = postRepo;
            _commentRepo = commentRepo;
        }
        
        
        
        [HttpGet("/api/posts/{id}/comments")]
        public async Task<IActionResult> Comments(int id)
        {
            if (await _postRepo.IsPostExists(id))
            {
                var comments = await _commentRepo.GetAll()
                    .Where(c => c.PostId == id)
                    .Include(x => x.Author).ToListAsync();
                return Ok(comments);
            }

            return BadRequest();
        }
        [Authorize]
        [HttpPost("/api/posts/{id}/comments")]
        
        public async Task<IActionResult> Comments(int id,CommentCreateDTO commentCreateDTO)
        {
            if (id != commentCreateDTO.PostId)
                return BadRequest();
            int userid = Convert.ToInt32(User.Claims.Where(x => x.Type == "UserId").FirstOrDefault()?.Value);

            var comment = Mapper.CommentCreateDTOToComment(commentCreateDTO);
            comment.AuthorId = userid;
            _commentRepo.Add(comment);

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
        [Authorize]
        [HttpPatch("/api/posts/{postId}/comments/{commentId}")]
        public async Task<IActionResult> Posts(int postId, int commentId, [FromBody] JsonPatchDocument<Comment> patchEntity)
        {

            if (patchEntity == null)
                return BadRequest();

            var postFromDB = await _postRepo.GetAll().FirstOrDefaultAsync(x => x.PostId == postId);
            int userid = Convert.ToInt32(User.Claims.Where(x => x.Type == "UserId").FirstOrDefault()?.Value);

            if (postFromDB == null)
                return NotFound();

            
            var commentFromDB = await _commentRepo.GetAll().FirstOrDefaultAsync(x => x.CommentId == commentId && x.PostId == postId);
            if (commentFromDB == null)
                return NotFound();

            //check if is the same owner of the post
            if (commentFromDB.AuthorId != userid)
                return Unauthorized("your'e not authorized to update this comment");

            patchEntity.ApplyTo(commentFromDB, ModelState);

            if (await _commentRepo.SaveAll())
                return Ok(commentFromDB);
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
        //[Authorize]
        [HttpDelete("/api/posts/{postId}/comments/{commentId}")]
        public async Task<IActionResult> Delete(int postId,int commentId)
        {
            int userid = Convert.ToInt32(User.Claims.Where(x => x.Type == "UserId").FirstOrDefault()?.Value);

            var commentFromDB = await _commentRepo.GetAll().FirstOrDefaultAsync(x => x.CommentId == commentId && x.PostId == postId);
            if (commentFromDB == null)
                return NotFound("");

            if(commentFromDB.AuthorId != userid)
                return BadRequest("cannot delete comment of someone else");

            var childrenCount = await _commentRepo.GetAll().Where(x => x.CommentParentId == commentId).CountAsync();
            if (childrenCount > 0)
                return BadRequest("cannot delete comment with children");

            _commentRepo.Delete(commentFromDB);
            if (await _commentRepo.SaveAll())
                return Ok();
            else
                return BadRequest();
        }




    }
}