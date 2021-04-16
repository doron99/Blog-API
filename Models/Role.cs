using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_API.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        [ForeignKey("Users")]
        public int UID { get; set; }
        public User User { get; set; }
        [MaxLength(50)]
        public string RoleName { get; set; }
    }
}
