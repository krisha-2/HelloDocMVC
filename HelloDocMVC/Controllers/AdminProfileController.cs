using HelloDocMVC.Entity.Models;
using HelloDocMVC.Models;
using HelloDocMVC.Repository.Repository;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HelloDocMVC.Controllers
{
    public class AdminProfileController : Controller
    {
        private readonly IAdminProfile _IAdminProfile;
        private readonly IComboBox _combobox;
        public AdminProfileController(IAdminProfile IMyProfileRepository, IComboBox combobox)
        {
            _IAdminProfile = IMyProfileRepository;
            _combobox = combobox;
        }
        public async Task<IActionResult> Index(int? id)
        {
            ViewAdminProfileData p = await _IAdminProfile.GetProfileDetails((id != null ? (int)id : Convert.ToInt32(CV.UserID())));
            ViewBag.RegionComboBox = _combobox.RegionComboBox();
            ViewBag.userrolecombobox = await _combobox.AdminRoleComboBox();
            return View("../AdminProfile/Index", p);
        }
        public async Task<IActionResult> EditPassword(string password)
        {
            if (await _IAdminProfile.EditPassword(password, Convert.ToInt32(CV.UserID())))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Password changed Successfully";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Password not Changed";
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult EditAdministratorInfo(ViewAdminProfileData _viewAdminProfile)
        {
            if (_IAdminProfile.EditAdministratorInfo(_viewAdminProfile))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Information changed Successfully";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Information not Changed";
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> BillingInfoEdit(ViewAdminProfileData _viewAdminProfile)
        {
            if (await _IAdminProfile.BillingInfoEdit(_viewAdminProfile))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Information changed Successfully";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Information not Changed";
            }
            return RedirectToAction("Index");
        }
    }
}
