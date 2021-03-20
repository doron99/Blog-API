using Blog_API.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blog_API.Helpers
{
    public static class JWTHelper
    {
        
        public static string createLoginToken(User user)
        {
            var _config = Startup.StaticConfig;
            var mySecret = _config.GetSection("AppSettings:Token").Value;
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            var myIssuer = _config.GetSection("AppSettings:Issuer").Value;
            var myAudience = _config.GetSection("AppSettings:Audience").Value;

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier,user.UID.ToString()),
                    new Claim(ClaimTypes.Email,user.Uemail.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = myIssuer,
                Audience = myAudience,
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
