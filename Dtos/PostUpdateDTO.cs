using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_API.Dtos
{
    public class PostUpdateDTO
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Excerpt { get; set; }
        public string CoverImagePath { get; set; }
        public bool Public { get; set; }
        public DateTime Updated { get; set; } = DateTime.Now;
        public bool Deleted { get; set; } = false;
        public int AuthorId { get; set; }
    }
}
