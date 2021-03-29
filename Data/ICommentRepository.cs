using Blog_API.Dtos;
using Blog_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_API.Data
{
    public interface ICommentRepository : IRepository<Comment>
    {
        
    }
}
