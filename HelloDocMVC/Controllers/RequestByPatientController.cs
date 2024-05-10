using HelloDocMVC.Entity.DataContext;
using Microsoft.AspNetCore.Mvc;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Models;
using System.Collections;
using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using HelloDocMVC.Repository.Repository;

namespace HelloDoc.Controllers
{
    public class RequestByPatientController : Controller
    {
        private readonly HelloDocDbContext _context;
        private readonly IPatientDashboard _patientDashboard;
        private readonly IRequestByPatient _requestByPatient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RequestByPatientController(HelloDocDbContext context, IPatientDashboard patientDashboard,IRequestByPatient requestByPatient, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _patientDashboard = patientDashboard;
            _requestByPatient = requestByPatient;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Me()
        {
            int id = Int32.Parse(CV.UserID());
            var ViewPatientCreateRequest = _requestByPatient.viewMeData(id);
            return View(ViewPatientCreateRequest);
        }
        [HttpPost]
        public IActionResult Me(Patient viewPatientReq)
        {
            _requestByPatient.meRequset(viewPatientReq);
            return RedirectToAction("Index", "PatientDashboard");
        }
        public IActionResult SomeoneElse()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SomeoneElse(FamilyFriend viewFamilyReq)
        {
            int id=Int32.Parse(CV.UserID());
            _requestByPatient.elseRequset(viewFamilyReq, id);
            return RedirectToAction("Index", "PatientDashboard");
        }       
    }

}
