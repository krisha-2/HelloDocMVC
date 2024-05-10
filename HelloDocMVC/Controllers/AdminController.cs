using HelloDocMVC.Entity.Models;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
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
using Rotativa;
using ViewAsPdf = Rotativa.AspNetCore.ViewAsPdf;


namespace HelloDocMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminDashboard _IAdminDashboard;
        private readonly IComboBox _comboBox;

        public AdminController(IAdminDashboard adminDashboard, IComboBox comboBox)
        {
            _IAdminDashboard = adminDashboard;
            _comboBox = comboBox;
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
            return View("../Admin/Index", sm);
        }
        public async Task<IActionResult> CreatNewAccontPost(string Email, string Password)
        {
            if (await _IAdminDashboard.CreatNewAccont(Email, Password))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "User Created Successfully";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Failed to Create User";
            }

            return View("../AdminLogin/Index");
        }
        public async Task<IActionResult> CreateAccount()
        {
            return View("../Admin/CreateAccount");
        }
        public IActionResult ViewCase(int? id)
        {
            ViewBag.RegionComboBox = _comboBox.RegionComboBox();
            var Data = _IAdminDashboard.ViewCaseData(id);
            return View(Data);
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
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Case Canceled Successfully";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Case Not Canceled";
            }
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult BlockCase(int RequestID, string Note)
        {
            bool BlockCase = _IAdminDashboard.BlockCase(RequestID, Note);
            if (BlockCase)
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Case Blocked Successfully";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Case Not BLocked";
            }
            return RedirectToAction("Index", "Admin");
        }
        public async Task<IActionResult> AssignProvider(int requestid, int ProviderId, string Notes)
        {
            if (await _IAdminDashboard.AssignProvider(requestid, ProviderId, Notes))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Physician Assigned successfully";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Physician Not Assigned";
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
                    // Success notification
                    TempData["SweetAlertType"] = "success";
                    TempData["SweetAlertMessage"] = "Order Created successfully";
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    // Error notification
                    TempData["SweetAlertType"] = "error";
                    TempData["SweetAlertMessage"] = "Order Not Created";
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
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Case Cleared...You can not show Cleared Case";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "there is some error in deletion";
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
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "File Uploaded Successfully";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "File Not Uploaded";
            }
            return RedirectToAction("ViewUploads", "Admin", new { id = Requestid });
        }
        public async Task<IActionResult> DeleteFile(int? id, int Requestid)
        {
            if (await _IAdminDashboard.DeleteDocumentByRequest(id.ToString()))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "File Deleted Successfully";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "File Not Deleted";
            }
            return RedirectToAction("ViewUploads", "Admin", new { id = Requestid });
        }
        public async Task<IActionResult> AllFilesDelete(string deleteids, int Requestid)
        {
            if (await _IAdminDashboard.DeleteDocumentByRequest(deleteids))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "All Selected File Deleted Successfully";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "All Selected File Not Deleted";
            }
            return RedirectToAction("ViewUploads", "Admin", new { id = Requestid });
        }
        public async Task<IActionResult> TransferProvider(int requestId, int ProviderId, string Notes)
        {
            if (await _IAdminDashboard.TransferProvider(requestId, ProviderId, Notes))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Physician Transfered successfully";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Physician Not Transfered";
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
                    // Success notification
                    TempData["SweetAlertType"] = "success";
                    TempData["SweetAlertMessage"] = "Notes Updated successfully";
                    return RedirectToAction("ViewNotes", new { id = RequestID });
                }
                else
                {
                    // Error notification
                    TempData["SweetAlertType"] = "error";
                    TempData["SweetAlertMessage"] = "Notes Not Updated";
                    return View("../Admin/ViewNotes");
                }
            }
            else
            {
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Please Select one of the note!!";
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
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Mail Send Successfully";
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
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Case Closed..You can see Closed case in unpaid State";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "there is some error in CloseCase";
            }
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult EditForCloseCase(ViewCloseCase sm)
        {
            bool result = _IAdminDashboard.EditForCloseCase(sm);

            if (result)
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Case Edited Successfully";
                return RedirectToAction("CloseCase", new { sm.RequestID });
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Case Not Edited";
                return RedirectToAction("CloseCase", new { sm.RequestID });
            }

        }
        public IActionResult EncounterModel(int id)
        {
            ViewEncounter data = _IAdminDashboard.GetEncounterDetails(id);
            return View("../Admin/EncounterForm", data);
        }
        public IActionResult EncounterEdit(ViewEncounter data)
        {
            if (_IAdminDashboard.EditEncounterDetails(data, CV.ID()))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Encounter Changes Saved";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Encounter Changes Not Saved";
            }
            return RedirectToAction("EncounterModel", new { id = data.Requesid });
        }
        public IActionResult Finalize(ViewEncounter model)
        {
            bool data = _IAdminDashboard.EditEncounterDetails(model, CV.ID());
            if (data)
            {
                bool final = _IAdminDashboard.CaseFinalized(model);
                if (final)
                {
                    // Success notification
                    TempData["SweetAlertType"] = "success";
                    TempData["SweetAlertMessage"] = "Case Is Finalized";
                    if (CV.role() == "Physician")
                    {
                        return Redirect("~/Physician/DashBoard");
                    }
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    // Error notification
                    TempData["SweetAlertType"] = "error";
                    TempData["SweetAlertMessage"] = "there is some error in CloseCase";
                    return RedirectToAction("EncounterModel", new { id = model.Requesid });
                }
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Case Is Not Finalized";
                return RedirectToAction("EncounterModel", new { id = model.Requesid });
            }

        }
        public IActionResult generatePDF(int id)
        {
            var FormDetails = _IAdminDashboard.GetEncounterDetails(id);
            return new ViewAsPdf("../Admin/EncounterPdf", FormDetails);
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
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Mail Send Successfully";
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
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Mail Send Successfully";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Mail is not send successfully";
            }
            return RedirectToAction("ViewUploads", "Admin", new { id = Requestid });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _AcceptRequestPost(int RequestId, string Note)
        {
            if (await _IAdminDashboard.AcceptPhysician(RequestId, Note, Convert.ToInt32(CV.UserID())))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Case Accepted";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Case Not Accepted";
            }
            return Redirect("~/Physician/Dashboard");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _TransfertoAdminPost(int RequestID, string Note)
        {
            if (await _IAdminDashboard.TransfertoAdmin(RequestID, Note, Convert.ToInt32(CV.UserID())))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Request Successfully transfered to admin";
            }

            return Redirect("~/Physician/Dashboard");
        }
        public IActionResult ContactAdmin(string Note)
        {
            bool Contact = _IAdminDashboard.ContactAdmin(Convert.ToInt32(CV.UserID()), Note);
            if (Contact)
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Mail Send Succesfully";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Mail Not Send Succesfully";
            }
            return RedirectToAction("PhysicianProfile", "Providers", new { id = Convert.ToInt32(CV.UserID()) });
        }
        public IActionResult Housecall(int RequestId)
        {
            if (_IAdminDashboard.Housecall(RequestId))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Case Accepted";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Case Not Accepted";
            }
            return Redirect("~/Physician/DashBoard");
        }
        public IActionResult Consult(int RequestId)
        {
            if (_IAdminDashboard.Consult(RequestId))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Case is in conclude state";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Error";
            }
            return Redirect("~/Physician/DashBoard");
        }
        public async Task<IActionResult> ConcludeCare(int? id, ViewDocuments viewDocument)
        {
            if (id == null)
            {
                id = viewDocument.RequestID;
            }
            ViewDocuments v = await _IAdminDashboard.GetDocumentByRequest(viewDocument, id);
            return View("../Admin/ConcludeCare", v);
        }
        public IActionResult UploadDocProvider(int Requestid, IFormFile file)
        {
            if (_IAdminDashboard.SaveDoc(Requestid, file))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "File Uploaded Successfully";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "File Not Uploaded";
            }
            return RedirectToAction("ConcludeCare", "Admin", new { id = Requestid });
        }
        public IActionResult ConcludeCarePost(int RequestId, string Notes)
        {
            if (_IAdminDashboard.ConcludeCarePost(RequestId, Notes))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Case concluded";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Error";
            }
            return Redirect("~/Physician/Dashboard");
        }
    }
}