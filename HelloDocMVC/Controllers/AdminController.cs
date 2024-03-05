using HelloDocMVC.Entity.Models;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreHero.ToastNotification.Abstractions;
using System.Diagnostics;
using System.Web;
using HelloDocMVC.Entity;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Models;
using Microsoft.EntityFrameworkCore;
using static HelloDocMVC.Entity.Models.Constant;
using HelloDocMVC.Repository.Repository;

namespace HelloDocMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminDashboard _IAdminDashboard;
        private readonly IComboBox _comboBox;
        private readonly INotyfService _notyf;

        public AdminController(IAdminDashboard adminDashboard, IComboBox comboBox, INotyfService notyf)
        {
            _IAdminDashboard = adminDashboard;
            _comboBox = comboBox;
            _notyf = notyf;
        }
        //public IActionResult Index()
        //{
        //    var Data = _IAdminDashboard.IndexData();
        //    return View(Data);
        //}
        public async Task<IActionResult> IndexAsync()
        {
            ViewBag.RegionComboBox =  _comboBox.RegionComboBox();
            ViewBag.CaseReasonComboBox = await _comboBox.CaseReasonComboBox();
            CountStatusWiseRequestModel sm = _IAdminDashboard.IndexData();
            return View(sm);
        }
        public IActionResult ViewCase(int? id)
        {
            ViewBag.RegionComboBox =  _comboBox.RegionComboBox();
            var Data = _IAdminDashboard.ViewCaseData(id);
            return View(Data);
        }
        public IActionResult ViewNotes()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _SearchResult(string Status)
        {
            if (Status == null)
            {
                Status = "1";
            }
            List<AdminDashboardList> contacts = _IAdminDashboard.GetRequests(Status);

            switch (Status)
            {
                case "1":
                    return PartialView("../Admin/_New", contacts);

                    break;
                case "2":

                    return PartialView("../Admin/_Pending", contacts);
                    break;
                case "4,5":

                    return PartialView("../Admin/_Active", contacts);
                    break;
                case "6":

                    return PartialView("../Admin/_Conclude", contacts);
                    break;
                case "3,7,8":

                    return PartialView("../Admin/_ToClose", contacts);
                    break;
                case "9":

                    return PartialView("../Admin/_UnPaid", contacts);
                    break;
            }
            return PartialView("../Admin/_New", contacts);
        }
        public IActionResult CancelCase(int RequestID, string Note, string CaseTag)
        {
            bool CancelCase = _IAdminDashboard.CancelCase(RequestID, Note, CaseTag);
            if (CancelCase)
            {
                _notyf.Success("Case Canceled Successfully");

            }
            else
            {
                _notyf.Error("Case Not Canceled");

            }
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult BlockCase(int RequestID, string Note)
        {
            bool BlockCase = _IAdminDashboard.BlockCase(RequestID, Note);
            if (BlockCase)
            {
                _notyf.Success("Case Blocked Successfully");
            }
            else
            {
                _notyf.Error("Case Not Blocked");
            }
            return RedirectToAction("Index", "Admin");
        }
        #region AssignProvider
        public async Task<IActionResult> AssignProvider(int requestid, int ProviderId, string Notes)
        {
            if (await _IAdminDashboard.AssignProvider(requestid, ProviderId, Notes))
            {
                _notyf.Success("Physician Assigned successfully...");
            }
            else
            {
                _notyf.Error("Physician Not Assigned...");
            }

            return RedirectToAction("Index", "Admin");
        }
        #endregion
        #region providerbyregion
        public IActionResult ProviderbyRegion(int Regionid)
        {
            var v = _comboBox.ProviderbyRegion(Regionid);
            return Json(v);
        }
        #endregion
        //#region Clear_case
        //public IActionResult ClearCase(int RequestID)
        //{
        //    bool cc = _IAdminDashboard.ClearCase(RequestID);
        //    if (cc)
        //    {
        //        _notyf.Success("Case Cleared...");
        //        _notyf.Warning("You can not show Cleared Case ...");
        //    }
        //    else
        //    {
        //        _notyf.Error("there is some error in deletion...");
        //    }
        //    return RedirectToAction("Index", "AdminDashBoard", new { Status = "2" });
        //}
        //#endregion
        public IActionResult ViewUpload(int RequestId)
        {
            var result = _IAdminDashboard.ViewUpload(RequestId);
            return View(result);
        }
        [HttpPost]
        public IActionResult ViewUpload(ViewDocument v, IFormFile? UploadFile)
        {
            _IAdminDashboard.UploadDoc(v, UploadFile);
            return ViewUpload(v.RequestId);
        }
    }
}
      
