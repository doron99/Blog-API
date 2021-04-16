using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_API.Dtos
{
    public class UserUpdateDTO
    {
        [Required]
        public int UID { get; set; }
        [MaxLength(30)]
        public string Uemail { get; set; }
        [Required]
        [MaxLength(30)]
        public string Ufname { get; set; }
        [Required]
        [MaxLength(30)]
        public string Ulname { get; set; }
        [Required]
        public bool isPost { get; set; }
        [Required]
        public bool isComment { get; set; }
    }
}
