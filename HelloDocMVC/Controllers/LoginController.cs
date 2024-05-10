using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using HelloDocMVC.Entity.DataContext;
//using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocMVC.Repository.Repository.Interface;
using HelloDocMVC.Repository.Repository;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Entity.DataModels;

namespace HelloDoc.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginRepository _ILoginRepository;
        private readonly HelloDocDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtSession _jwtSession;
        public LoginController(ILoginRepository loginRepository,HelloDocDbContext context, IHttpContextAccessor httpContextAccessor, IJwtSession jwtSession)
        {
            _ILoginRepository = loginRepository;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _jwtSession = jwtSession;
        }
        public IActionResult Index()
        {
            return View( );
        }
        public IActionResult Forgetpass()
        {
            return View("../Login/Forgetpass");
        }
        [HttpPost]
        public async Task<IActionResult> Index(string Email, string PasswordHash)
        {
            var user = new AspNetUser();
            user.Email = Email;
            user.PasswordHash = PasswordHash;
            UserInfo u = await _ILoginRepository.CheckAccessLogin(user);
            if (user == null)
            {
                ViewData["Error"] = " Your Username or password is incorrect. ";
                return View("../Login/Index");
            }
            else
            {
                var jwtToken = _jwtSession.GenerateJWTAuthetication(u);
                Response.Cookies.Append("jwt", jwtToken);
                //int id = _context.Users.FirstOrDefault(u => u.AspNetUserId == user.Id).UserId;
                //string userName = _context.Users.Where(x => x.AspNetUserId == user.Id).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
                //_httpContextAccessor.HttpContext.Session.SetInt32("id", id);
                //_httpContextAccessor.HttpContext.Session.SetString("Name", userName);
                return RedirectToAction("Index", "PatientDashboard");
            }
            }
        public IActionResult ForgotPass()
        {
            return View();
        }
        public IActionResult ResetEmail(string Email)
        {
            if (_ILoginRepository.SendResetLink(Email))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Mail Send  Successfully";
            }
            return RedirectToAction("Forgetpass", "Login");

        }
        [HttpGet]
        public IActionResult ResetPass(string email, string datetime)
        {
            TempData["email"] = email;
            //TimeSpan time = DateTime.Now - DateTime.Parse(datetime);
            //if (time.TotalHours > 24)
            //{
            //    return View("LinkExpired");
            //}
            //else
            //{
                return View();
            //}
        }
        [HttpPost]
        public IActionResult SavePassword(Patient viewPatientReq)
        {
            var aspnetuser = _context.AspNetUsers.FirstOrDefault(m => m.Email == viewPatientReq.Email);
            if (aspnetuser != null)
            {
                aspnetuser.PasswordHash = viewPatientReq.PassWord;
                _context.AspNetUsers.Update(aspnetuser);
                _context.SaveChangesAsync();

                TempData["emailmessage"] = "Your password is changed!!";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                TempData["emailmessage"] = "Email is not registered!!";
                return View("ResetPass");
            }
        }
        public IActionResult Logout()
            {
            //HttpContext.Session.Clear();
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Index");
            }

    }
}
