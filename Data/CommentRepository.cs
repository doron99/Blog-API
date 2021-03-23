using Blog_API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_API.Data
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DataContext _db;

        public CommentRepository(DataContext db)
        {
            _db = db;
        }

        public void Add(Comment item)
        {
            _db.Add(item);
        }

        public void Delete(Comment item)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Comment> GetAll()
        {
            return _db.Comments.AsQueryable();
        }

        public async Task<bool> SaveAll()
        {
            return await _db.SaveChangesAsync() > 0 ? true : false;
        }

        public void Update(Comment item)
        {
            throw new NotImplementedException();
        }

        //public async Task<List<Post>> getPosts()
        //{
        //    var posts = await GetAll().AsQueryable().ToListAsync();
        //    return posts;
        //}

        //public void Add(Post item)
        //{
        //    _db.Add(item);
        //}

        //public void Delete(Post item)
        //{
        //    throw new NotImplementedException();
        //}

        //public IQueryable<Post> GetAll()
        //{
        //    return _db.Posts.AsQueryable();
        //}

        //public async Task<bool> SaveAll()
        //{
        //    return await _db.SaveChangesAsync() > 0 ? true : false;
        //}

        //public void Update(Post item)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
