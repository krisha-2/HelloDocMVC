using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HelloDocMVC.Repository.Repository.Interface;

namespace HelloDocMVC.Controllers
{
    [AttributeUsage(AttributeTargets.All)]
    public class CheckProviderAccess : Attribute, IAuthorizationFilter
    {
        private readonly List<string> _role;
        public CheckProviderAccess(string role = "")
        {
            _role = role.Split(',').ToList();
        }
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var jwtservice = filterContext.HttpContext.RequestServices.GetService<IJwtSession>();
            if (jwtservice == null)
            {
                filterContext.Result = new RedirectResult("../AdminLogin/Index");
                return;
            }
            var request = filterContext.HttpContext.Request;
            var toket = request.Cookies["jwt"];
            if (toket == null || !jwtservice.ValidateToken(toket, out JwtSecurityToken jwtSecurityTokenHandler))
            {
                filterContext.Result = new RedirectResult("../AdminLogin/Index");
                return;
            }
            var roles = jwtSecurityTokenHandler.Claims.FirstOrDefault(claiim => claiim.Type == ClaimTypes.Role);

            if (roles == null)
            {
                filterContext.Result = new RedirectResult("../AdminLogin/Index");
                return;
            }

            var flag = false;
            foreach (var role in _role)
            {
                if (string.IsNullOrWhiteSpace(role) || roles.Value != role)
                {
                    flag = false;
                }
                else
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                filterContext.Result = new RedirectResult("../AdminLogin/AuthError");

            }

        }

    }
}
