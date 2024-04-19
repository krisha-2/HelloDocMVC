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
using OfficeOpenXml;

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
        [CheckProviderAccess("Admin,Physician")]
        [Route("Physician/Dashboard")]
        [Route("Admin/Dashboard")]
        public async Task<IActionResult> IndexAsync()
        {
            ViewBag.Profession = _comboBox.Profession();
            ViewBag.RegionComboBox = _comboBox.RegionComboBox();
            ViewBag.CaseReasonComboBox = await _comboBox.CaseReasonComboBox();
            //PaginatedViewModel sm = _IAdminDashboard.IndexData();
            PaginatedViewModel sm = _IAdminDashboard.IndexData(-1);
            if (CV.role() == "Physician")
            {
                sm = _IAdminDashboard.IndexData(Convert.ToInt32(CV.UserID()));
            }
            return View("../Admin/Index",sm);
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

            if (CV.role() == "Physician")
            {
                contacts = _IAdminDashboard.GetRequests(Status, data, Convert.ToInt32(CV.UserID()));
            }
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
        public IActionResult ProviderbyRegion(int Regionid)
        {
            var v = _comboBox.ProviderbyRegion(Regionid);
            return Json(v);
        }
        public IActionResult Orders()
        {
            ViewBag.Profession = _comboBox.Profession();
           
            return View();
        }
        [HttpPost]
        public IActionResult Orders(Orders sm)
        {
            if (ModelState.IsValid)
            {
                bool data = _IAdminDashboard.SendOrder(sm);
                if (data)
                {
                    _notyf.Success("Order Created  successfully...");
                    _notyf.Information("Mail is sended to Vendor successfully...");
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    _notyf.Error("Order Not Created...");
                    return View("../Admin/Orders", sm);
                }
            }
            else
            {
                return View("../Admin/Orders", sm);
            }
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
        public async Task<IActionResult> ViewUploads(ViewDocuments viewDocument, int? id)
        {
            ViewDocuments v = await _IAdminDashboard.GetDocumentByRequest(viewDocument, id);
            return View("../Admin/ViewUploads", v);
        }
        public IActionResult UploadDoc(int Requestid, IFormFile file)
        {
            if (_IAdminDashboard.SaveDoc(Requestid, file))
            {
                _notyf.Success("File Uploaded Successfully");
            }
            else
            {
                _notyf.Error("File Not Uploaded");
            }
            return RedirectToAction("ViewUploads", "Admin", new { id = Requestid });
        }
        public async Task<IActionResult> DeleteFile(int? id, int Requestid)
        {
            if (await _IAdminDashboard.DeleteDocumentByRequest(id.ToString()))
            {
                _notyf.Success("File Deleted Successfully");
            }
            else
            {
                _notyf.Error("File Not Deleted");
            }
            return RedirectToAction("ViewUploads", "Admin", new { id = Requestid });
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
            return RedirectToAction("ViewUploads", "Admin", new { id = Requestid });
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
        public IActionResult ViewNotes(int id)
        {

            ViewNotesData sm = _IAdminDashboard.getNotesByID(id);
            return View("../Admin/ViewNotes", sm);
        }
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
        public IActionResult Export(string status)
        {
            var requestData = _IAdminDashboard.Export(status);

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (ExcelPackage package = new())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("RequestData");

                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Requestor";
                worksheet.Cells[1, 3].Value = "Request Date";
                worksheet.Cells[1, 4].Value = "Phone";
                worksheet.Cells[1, 5].Value = "Address";
                worksheet.Cells[1, 6].Value = "Notes";
                worksheet.Cells[1, 7].Value = "Physician";
                worksheet.Cells[1, 8].Value = "Birth Date";
                worksheet.Cells[1, 9].Value = "RequestTypeId";
                worksheet.Cells[1, 10].Value = "Email";
                worksheet.Cells[1, 11].Value = "RequestId";

                for (int i = 0; i < requestData.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = requestData[i].PatientName;
                    worksheet.Cells[i + 2, 2].Value = requestData[i].Requestor;
                    worksheet.Cells[i + 2, 3].Value = requestData[i].RequestedDate;
                    worksheet.Cells[i + 2, 4].Value = requestData[i].PatientPhoneNumber;
                    worksheet.Cells[i + 2, 5].Value = requestData[i].Address;
                    worksheet.Cells[i + 2, 6].Value = requestData[i].Notes;
                    worksheet.Cells[i + 2, 7].Value = requestData[i].ProviderName;
                    worksheet.Cells[i + 2, 8].Value = requestData[i].DateOfBirth;
                    worksheet.Cells[i + 2, 9].Value = requestData[i].RequestTypeId;
                    worksheet.Cells[i + 2, 10].Value = requestData[i].Email;
                    worksheet.Cells[i + 2, 11].Value = requestData[i].RequestId;
                }

                byte[] excelBytes = package.GetAsByteArray();

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
        [HttpPost]
        public IActionResult SendLink(string FirstName, string LastName, string Email)
        {
            if (_IAdminDashboard.SendLink(FirstName, LastName, Email))
            {
                _notyf.Success("Mail Send  Successfully..!");
            }
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult CreateRequest()
        {
            return View("./CreateRequest");
        }
        [HttpPost]
        public IActionResult CreateRequest(Patient vdcp)
        {
            _IAdminDashboard.CreateRequest(vdcp);
            return RedirectToAction("Index", "Admin");
        }
        public async Task<IActionResult> SendFileEmail(string mailids, int Requestid, string email)
        {
            if (await _IAdminDashboard.SendFileEmail(mailids, Requestid, email))
            {

                _notyf.Success("Mail Send successfully");
            }
            else
            {
                _notyf.Error("Mail is not send successfully");
            }
            return RedirectToAction("ViewUploads", "Admin", new { id = Requestid });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _AcceptRequestPost(int RequestId, string Note)
        {
            if (await _IAdminDashboard.AcceptPhysician(RequestId, Note, Convert.ToInt32(CV.UserID())))
            {
                _notyf.Success("Case Accepted...");
            }
            else
            {
                _notyf.Success("Case Not Accepted...");
            }
            return Redirect("~/Physician/Dashboard");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _TransfertoAdminPost(int RequestID, string Note)
        {
            if (await _IAdminDashboard.TransfertoAdmin(RequestID, Note, Convert.ToInt32(CV.UserID())))
            {
                _notyf.Success("Request Successfully transfered to admin...");
            }

            return Redirect("~/Physician/Dashboard");
        }
    }
}
    
