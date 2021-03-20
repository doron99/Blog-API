using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_API.Dtos
{
    public class UserDTO
    {
        public int UID { get; set; }
        public string Uemail { get; set; }
        public string Ufname { get; set; }
        public string Ulname { get; set; }
        public DateTime Created { get; set; }
        public string Uimg { get; set; }
    }
}
