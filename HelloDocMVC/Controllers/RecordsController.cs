using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Repository.Repository;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HelloDocMVC.Controllers
{
    public class RecordsController : Controller
    {
        private readonly HelloDocDbContext _context;
        private readonly IRecords _IRecords;
        public RecordsController(HelloDocDbContext context, IRecords Records)
        {
            _context = context;
            _IRecords = Records;
        }
        public IActionResult PatientRecord(string firstname, string lastname, string email, string phone)
        {
            var res = _IRecords.PatientHistory(firstname, lastname, email, phone);
            return View("PatientRecord", res);
        }
        public IActionResult Explore(int UserId)
        {
            var res = _IRecords.RecordsPatientExplore(UserId);
            return View(res);
        }
    }
}
