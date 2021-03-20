using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_API.Dtos
{
    public class UserLoginDTO
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}
