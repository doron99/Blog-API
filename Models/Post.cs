using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_API.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }
        public int Views { get; set; } = 0;
        [Required]
        [MaxLength(2000)]
        public string Content { get; set; }
        [Required]
        [MaxLength(255)]
        public string Excerpt { get; set; }
        [MaxLength(100)]
        public string CoverImagePath { get; set; }
        public bool Public { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; } = DateTime.Now;
        public bool Deleted { get; set; } = false;

        public int AuthorId { get; set; }
        public User Author { get; set; }
        
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
