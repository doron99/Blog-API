using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_API.Models
{
    public class User
    {
        [Key]
        public int UID { get; set; }
        [Required]
        [MaxLength(30)]
        public string Uemail { get; set; }
        [Required]
        [MaxLength(30)]
        public string Ufname { get; set; }
        [Required]
        [MaxLength(30)]
        public string Ulname { get; set; }
        [JsonIgnore]
        public byte[] PasswordHash { get; set; }
        [JsonIgnore]
        public byte[] PasswordSalt { get; set; }
        public DateTime Created { get; set; }
        [MaxLength(50)]
        public string Uimg { get; set; }
        [JsonIgnore]
        public virtual ICollection<Post> Posts { get; set; }
        [JsonIgnore]
        public virtual ICollection<Comment> Comments {get; set;}

    }  
}        
