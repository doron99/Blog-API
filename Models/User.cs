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
        public string Uemail { get; set; }
        public string Ufname { get; set; }
        public string Ulname { get; set; }
        [JsonIgnore]
        public byte[] PasswordHash { get; set; }
        [JsonIgnore]
        public byte[] PasswordSalt { get; set; }
        public DateTime Created { get; set; }
        public string Uimg { get; set; }

    }  
}        
