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
        public IActionResult SearchRecord(RecordsData rm)
        {
            RecordsData model = _IRecords.GetFilteredSearchRecords(rm);
            return View("../Records/SearchRecord", model);
        }
        public IActionResult DeleteRequest(int? RequestId)
        {
            if (_IRecords.DeleteRequest(RequestId))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Request Deleted Successfully";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Request Not deleted";
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
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Case Unblocked Successfully";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Case remains blocked";
            }
            return RedirectToAction("BlockHistory");
        }
        public IActionResult EmailLogs(RecordsData rm)
        {
            RecordsData r = _IRecords.GetFilteredEmailLogs(rm);
            return View("../Records/EmailLogs", r);
        }
        public IActionResult SMSLogs(RecordsData rm)
        {
            RecordsData r = _IRecords.GetFilteredSMSLogs(rm);
            return PartialView("../Records/SMSLogs", r);
        }

    }
}
