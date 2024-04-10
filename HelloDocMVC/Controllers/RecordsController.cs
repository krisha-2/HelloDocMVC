using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Models;
using HelloDocMVC.Repository.Repository;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HelloDocMVC.Controllers
{
    public class RecordsController : Controller
    {
        private readonly HelloDocDbContext _context;
        private readonly IRecords _IRecords;
        private readonly INotyfService _notyf;
        public RecordsController(HelloDocDbContext context, IRecords Records, INotyfService notyf)
        {
            _context = context;
            _IRecords = Records;
            _notyf = notyf;
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
        public IActionResult SearchRecord(RecordsData rm)
        {
            RecordsData model = _IRecords.GetFilteredSearchRecords(rm);
            return View("../Records/SearchRecord", model);
        }
        public IActionResult DeleteRequest(int? RequestId)
        {
            if (_IRecords.DeleteRequest(RequestId))
            {
                _notyf.Success("Request Deleted Successfully.");
            }
            else
            {
                _notyf.Error("Request not deleted");
            }
            return RedirectToAction("SearchRecord");
        }
        public IActionResult BlockHistory(RecordsData rm)
        {
            RecordsData r = _IRecords.BlockHistory(rm);
            return PartialView("../Records/BlockHistory", r);
        }
        public IActionResult Unblock(int RequestId)
        {
            if (_IRecords.Unblock(RequestId, CV.ID()))
            {
                _notyf.Success("Case Unblocked Successfully.");
            }
            else
            {
                _notyf.Error("Case remains blocked.");
            }

            return RedirectToAction("BlockHistory");
        }
    }
}
