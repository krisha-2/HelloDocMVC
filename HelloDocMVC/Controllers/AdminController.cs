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
        [CheckProviderAccess("Admin")]
        public async Task<IActionResult> IndexAsync()
        {
            ViewBag.Profession = _comboBox.Profession();
            ViewBag.RegionComboBox =  _comboBox.RegionComboBox();
            ViewBag.CaseReasonComboBox = await _comboBox.CaseReasonComboBox();
            PaginatedViewModel sm = _IAdminDashboard.IndexData();
            return View(sm);
        }
        public IActionResult ViewCase(int? id)
        {
            ViewBag.RegionComboBox =  _comboBox.RegionComboBox();
            var Data = _IAdminDashboard.ViewCaseData(id);
            return View(Data);
        }
        public IActionResult Encounter(int id)
        {
            ViewEncounter ei = _IAdminDashboard.EncounterInfo(id);
            return View(ei);
        }
        [HttpPost]
        public IActionResult EncounterPost(ViewEncounter _viewencounterinfo)
        {
            if (ModelState.IsValid)
            {
                _IAdminDashboard.EditEncounterinfo(_viewencounterinfo);
                return RedirectToAction("Index", "Admin");

            }
            return View(_viewencounterinfo);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _SearchResult(PaginatedViewModel data, String Status)
        {
            if (Status == null)
            {
                Status = CV.CurrentStatus();

            }

            Response.Cookies.Delete("Status");
            Response.Cookies.Append("Status", Status);
            PaginatedViewModel contacts = _IAdminDashboard.GetRequests(Status, data);

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
        public IActionResult Orders()
        {
            ViewBag.Profession = _comboBox.Profession();
           
            return View();
        }
        public IActionResult Business(int profession)
        {
            var v = _comboBox.Business(profession);
            return Json(v);
        } 
        public IActionResult OrderData(int profession)
        {
            var v = _comboBox.OrderData(profession);
            return Json(v);
        }
        #region Clear_case
        public IActionResult ClearCase(int RequestID)
        {
            bool cc = _IAdminDashboard.ClearCase(RequestID);
            if (cc)
            {
                _notyf.Success("Case Cleared...");
                _notyf.Warning("You can not show Cleared Case ...");
            }
            else
            {
                _notyf.Error("there is some error in deletion...");
            }
            return RedirectToAction("Index", "Admin", new { Status = "2" });
        }
        #endregion
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
        public async Task<IActionResult> AllFilesDelete(string deleteids, int Requestid)
        {
            if (await _IAdminDashboard.DeleteDocumentByRequest(deleteids))
            {
                _notyf.Success("All Selected File Deleted Successfully");
            }
            else
            {
                _notyf.Error("All Selected File Not Deleted");
            }
            return RedirectToAction("ViewUpload", "Home", new { id = Requestid });
        }
        public async Task<IActionResult> TransferProvider(int requestId, int ProviderId, string Notes)
        {
            if (await _IAdminDashboard.TransferProvider(requestId, ProviderId, Notes))
            {
                _notyf.Success("Physician Transfered successfully...");
            }
            else
            {
                _notyf.Error("Physician Not Transfered...");
            }

            return RedirectToAction("Index", "Admin");
        }
        #region View_Notes
        public IActionResult ViewNotes(int id)
        {

            ViewNotesData sm = _IAdminDashboard.getNotesByID(id);
            return View("../Admin/ViewNotes", sm);
        }
        #endregion

        #region Edit_Notes
        public IActionResult ChangeNotes(int RequestID, string? adminnotes, string? physiciannotes)
        {
            if (adminnotes != null || physiciannotes != null)
            {
                bool result = _IAdminDashboard.EditViewNotes(adminnotes, physiciannotes, RequestID);
                if (result)
                {
                    _notyf.Success("Notes Updated successfully...");
                    return RedirectToAction("ViewNotes", new { id = RequestID });
                }
                else
                {
                    _notyf.Error("Notes Not Updated");
                    return View("../Admin/ViewNotes");
                }
            }
            else
            {
                _notyf.Information("Please Select one of the note!!");
                TempData["Errormassage"] = "Please Select one of the note!!";
                return RedirectToAction("ViewNotes", new { id = RequestID });
            }
        }
        #endregion
        //public IActionResult Orders(Orders sm)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        bool data = _IAdminDashboard.Orders(sm);
        //        if (data)
        //        {
        //            _notyf.Success("Order Created  successfully...");
        //            _notyf.Information("Mail is sended to Vendor successfully...");
        //            return RedirectToAction("Index", "AdminDashBoard");
        //        }
        //        else
        //        {
        //            _notyf.Error("Order Not Created...");
        //            return View("../Admin/Orders", sm);
        //        }
        //    }
        //    else
        //    {
        //        return View("../Admin/Orders", sm);
        //    }
        //}
        #region SendAgreement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendAgreementmail(int requestid)
        {
            if (_IAdminDashboard.SendAgreement(requestid))
            {
                _notyf.Success("Mail Send  Successfully..!");
            }
            return RedirectToAction("Index", "Admin");
        }
        #endregion
        #region CloseCase
        public async Task<IActionResult> CloseCase(int RequestID)
        {
            ViewCloseCase vc = _IAdminDashboard.CloseCaseData(RequestID);
            return View("../Admin/CloseCase", vc);
        }
        public IActionResult CloseCaseUnpaid(int id)
        {
            bool sm = _IAdminDashboard.CloseCase(id);
            if (sm)
            {
                _notyf.Success("Case Closed...");
                _notyf.Information("You can see Closed case in unpaid State...");

            }
            else
            {
                _notyf.Error("there is some error in CloseCase...");
            }
            return RedirectToAction("Index", "Admin");
        }
        #endregion

        public IActionResult EditForCloseCase(ViewCloseCase sm)
        {
            bool result = _IAdminDashboard.EditForCloseCase(sm);

            if (result)
            {
                _notyf.Success("Case Edited Successfully..");
                return RedirectToAction("CloseCase", new { sm.RequestID });
            }
            else
            {
                _notyf.Error("Case Not Edited...");
                return RedirectToAction("CloseCase", new { sm.RequestID });
            }

        }
    }
}
    
