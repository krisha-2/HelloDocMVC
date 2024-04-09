using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HelloDocMVC.Controllers
{
    public class AgreementPage : Controller
    {
        private readonly IAdminDashboard _IAdminDashboard;
        private readonly INotyfService _notyf;
        public AgreementPage(IAdminDashboard adminDashboard, IComboBox comboBox, INotyfService notyf)
        {
            _IAdminDashboard = adminDashboard;
            _notyf = notyf;
        }
        public IActionResult Index(int RequestID)
        {
            TempData["RequestID"] = " " + RequestID;
            TempData["PatientName"] = "krisha lukka";
            return View();
        }
        public IActionResult accept(int RequestID)
        {
            _IAdminDashboard.SendAgreement_accept(RequestID);
            return RedirectToAction("Index", "Admin");
        }
        public IActionResult Reject(int RequestID, string Notes)
        {
            _IAdminDashboard.SendAgreement_Reject(RequestID, Notes);
            return RedirectToAction("Index", "Admin");
        }
    }
}
