using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace HelloDocMVC.Controllers
{
    public class AdminLogin : Controller
    {
        private readonly HelloDocDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoginRepository _loginRepository;
        private readonly IJwtSession _jwtSession;
        public AdminLogin(HelloDocDbContext context, IHttpContextAccessor httpContextAccessor, ILoginRepository loginRepository, IJwtSession jwtSession)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _loginRepository = loginRepository;
            _jwtSession = jwtSession;
        }
        public IActionResult Index()
        {
            return View("../AdminLogin/Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Validate(AspNetUser aspNetUser)
        {
            UserInfo u = await _loginRepository.CheckAccessLogin(aspNetUser);

            if (u != null)
            {
                var jwttoken = _jwtSession.GenerateJWTAuthetication(u);
                Response.Cookies.Append("jwt", jwttoken);
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                ViewData["error"] = "Invalid Id Pass";
                return View("../AdminLogin/Index");
            }
        }
        #region end_session
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Index", "AdminLogin");
        }
        #endregion
        public IActionResult AuthError()
        {
            return View("../AdminLogin/AuthError");
        }
    }
}