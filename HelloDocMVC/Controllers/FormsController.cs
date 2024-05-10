
using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Repository.Repository;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;

namespace HelloDoc.Controllers
{
    public class FormsController : Controller
    {
        private readonly HelloDocDbContext _context;
        private readonly IPatientForms _patientForms;
        private readonly IComboBox _comboBox;
        public FormsController(HelloDocDbContext context, IPatientForms patientForms, IComboBox comboBox)
        {
            _context = context;
            _patientForms = patientForms;
            _comboBox = comboBox;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
        public IActionResult Patientform()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Patientform(Patient viewdata)
        {
            _patientForms.PatientRequest(viewdata);
            return View("../Forms/Index");
        }
        public IActionResult FamilyFriend()
        {
            return View();
        }
        [HttpPost]
        public IActionResult FamilyFriend(FamilyFriend viewdata)
        {
            _patientForms.FamilyFriendRequest(viewdata);
            return View("../Forms/Index");
        }
        public IActionResult Concierge()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Concierge(Concierge viewdata)
        {
            _patientForms.ConciergeRequest(viewdata);
            return View("../Forms/Index");
        }
        public IActionResult BusinessPartner()
        {
            return View();
        }
        [HttpPost]
        public IActionResult BusinessPartner(Business viewdata)
        {
            _patientForms.BusinessRequest(viewdata);
            return View("../Forms/Index");
        }
        [HttpPost]
        public async Task<IActionResult> CheckEmailAsync(string email)
        {
            string message;
            var aspnetuser = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Email == email);
            Console.Write(aspnetuser);
            if (aspnetuser == null)
            {
                message = "False";
            }
            else
            {
                message = "success";
            }
            return Json(new
            {
                isAspnetuser = aspnetuser == null
            });
        }
    }
}