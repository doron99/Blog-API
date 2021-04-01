using Blog_API.Dtos;
using Blog_API.Helpers;
using Blog_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Blog_API.Mappers
{
    public static class Mapper
    {
        public static User UserRegisterDTOToUser(UserRegisterDTO u)
        {
            return new User
            {
                Created = DateTime.Now,
                Uemail = u.email,
                Ufname = u.fname,
                Ulname = u.lname
            };
        }
        public static UserDTO UserToUserDTO(User u)
        {
            return new UserDTO
            {
                Uemail = u.Uemail,
                Ufname = u.Ufname,
                Ulname = u.Ulname,
                Created = u.Created,
                UID = u.UID,
                Uimg = u.Uimg
            };
        }
       
        //public static Expression<Func<User, UserDTO>> ToDTO()
        //{
        //    Expression<Func<User, UserDTO>> exp =
        //        u => new UserDTO
        //        {
        //            Uemail = u.Uemail,
        //            Ufname = u.Ufname,
        //            Ulname = u.Ulname,
        //            Created = u.Created,
        //            UID = u.UID,
        //            Uimg = u.Uimg
        //        };
        //    return exp;
        //}
        public static Post PostCreateDTOToPost(PostCreateDTO p)
        {
            return new Post
            {
                Created = DateTime.Now,
                AuthorId = Helper.IsNumeric(p.AuthorId)?p.AuthorId : -1,
                Content = p.Content,
                Deleted = false,
                Excerpt = p.Excerpt,
                Title = p.Title,
                Public = true
            };
        }
        public static Post PostUpdateDTOToPost(PostUpdateDTO p)
        {
            return new Post
            {
                Created = DateTime.Now,
                AuthorId = Helper.IsNumeric(p.AuthorId)?p.AuthorId : -1,
                Content = p.Content,
                Deleted = false,
                Excerpt = p.Excerpt,
                Title = p.Title,
                Public = true
            };
        }
        public static Comment CommentCreateDTOToComment(CommentCreateDTO cc)
        {
            return new Comment
            {
                Created = DateTime.Now,
                Content = cc.Content,
                PostId = cc.PostId,
                Public = true,
                CommentParentId = cc.CommentParentId
            };
        }

        public static void setPostUpdateFields(this Post post,PostUpdateDTO postDTO)
        {
            post.Updated = DateTime.Now;
            post.Content = postDTO.Content;
            post.Deleted = postDTO.Deleted;
            post.Excerpt = postDTO.Excerpt;
            post.Title = postDTO.Title;
        }

    }
}
