using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;
using System.IO;

namespace HelloDocMVC.Models
{
    public static class CV
    {
        private static IHttpContextAccessor _httpContextAccessor;

        static CV()
        {
            _httpContextAccessor = new HttpContextAccessor();
        }
        public static string? role()
        {
            string cookieValue;
            string role = null;

            if (_httpContextAccessor.HttpContext.Request.Cookies["jwt"] != null)
            {
                cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();

                role = DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "Role").Value;
            }

            return role;
        }


        public static string? UserName()
        {
            string cookieValue;
            string UserName = null;

            if (_httpContextAccessor.HttpContext.Request.Cookies["jwt"] != null)
            {
                cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();

                UserName = DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "Username").Value;
            }

            return UserName;
        }

        public static string? UserID()
        {
            string cookieValue;
            string UserID = null;

            if (_httpContextAccessor.HttpContext.Request.Cookies["jwt"] != null)
            {
                cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();

                UserID = DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "UserID").Value;
            }

            return UserID;
        }

        public static string? ID()
        {
            string cookieValue;
            string UserID = null;

            if (_httpContextAccessor.HttpContext.Request.Cookies["jwt"] != null)
            {
                cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["jwt"].ToString();

                UserID = DecodedToken.DecodeJwt(DecodedToken.ConvertJwtStringToJwtSecurityToken(cookieValue)).claims.FirstOrDefault(t => t.Key == "UserID").Value;
            }

            return UserID;
        }
        public static string? CurrentStatus()
        {
            string? Status = _httpContextAccessor.HttpContext.Request.Cookies["Status"];
            return Status;
        }

    }
}