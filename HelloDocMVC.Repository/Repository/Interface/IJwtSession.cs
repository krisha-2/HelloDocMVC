using HelloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Repository.Repository.Interface
{
    public interface IJwtSession
    {
        string GenerateJWTAuthetication(UserInfo userinfo);
        bool ValidateToken(string token, out JwtSecurityToken jwtSecurityTokenHandler);
    }
}
