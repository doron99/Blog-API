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
        
        public static string createLoginToken(User user,DateTime expiration )
        {
            var _config = Startup.StaticConfig;
            var mySecret = _config.GetSection("AppSettings:Token").Value;
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            var myIssuer = _config.GetSection("AppSettings:Issuer").Value;
            var myAudience = _config.GetSection("AppSettings:Audience").Value;

            var claims = new List<Claim> {
                            new Claim("UserId", user.UID.ToString()),
                            new Claim(ClaimTypes.Name, user.Ufname + " " + user.Ulname),
                            new Claim(ClaimTypes.Email, user.Uemail.ToString())};
            foreach (var role in user.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role.RoleName));

            var creds = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                                        issuer: myIssuer,
                                        audience:myAudience,
                                        claims: claims,
                                        expires:expiration,
                                        signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;

        }
    }
}
