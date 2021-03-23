using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_API.Dtos
{
    public class CommentCreateDTO
    {
        public string Content { get; set; }
        public int PostId { get; set; }
    }
}
