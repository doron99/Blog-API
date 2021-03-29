using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_API.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        [Required]
        [MaxLength(300)]
        public string Content { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public int Like { get; set; } = 0;
        public int Dislike { get; set; } = 0;
        public bool Public { get; set; } = false;
        public int? CommentParentId { get; set; } = -1;

        public int PostId { get; set; }
        [JsonIgnore]
        public virtual Post Post { get; set; }

        public int AuthorId { get; set; }
        public virtual User Author { get; set; }
        

    }
}
