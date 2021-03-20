using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog_API.Data;
using Blog_API.Dtos;
using Blog_API.Mappers;
using Blog_API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostRepository _postRepository;

        public PostsController(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Posts()
        {
            var list = await _postRepository.GetAll().AsQueryable().Include(x => x.Author).ToListAsync();
            return Ok(list);
        }
        [HttpPost]
        public async Task<IActionResult> Posts(PostCreateDTO postCreateDTO)
        {
            var post = Mapper.PostCreateDTOToPost(postCreateDTO);
            _postRepository.Add(post);
            if (await _postRepository.SaveAll())
                return Created("/","");
            else
                return BadRequest();
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> Posts(int id, [FromBody] JsonPatchDocument<Post> patchEntity)
        {

            if (patchEntity == null)
                return BadRequest();

            var postFromDB = await _postRepository.GetAll().FirstOrDefaultAsync(x => x.PostId == id);

            if (postFromDB == null)
                return NotFound();

            patchEntity.ApplyTo(postFromDB, ModelState);

            if (await _postRepository.SaveAll())
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

            var postFromDB = await _postRepository.GetAll().FirstOrDefaultAsync(x => x.PostId == id);

            if (postFromDB == null)
                return NotFound();

            //the function setPostUpdateFields is in mapper class.
            postFromDB.setPostUpdateFields(postUpdateDTO);

            _postRepository.Update(postFromDB);
            if (await _postRepository.SaveAll())
                return NoContent();
            else
                return BadRequest();


          
         
        }

    }
}