using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Models;
using HelloDocMVC.Repository.Repository;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HelloDocMVC.Controllers
{
    public class AdminProfileController : Controller
    {
        #region Constructor
        private readonly IAdminProfile _IAdminProfile;
        private readonly IComboBox _combobox;
        private readonly INotyfService _notyf;
        public AdminProfileController(IAdminProfile IMyProfileRepository, IComboBox combobox, INotyfService INotyfService)
        {
            _IAdminProfile = IMyProfileRepository;
            _combobox = combobox;
            _notyf = INotyfService;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(int? id)
        {
            ViewAdminProfileData p = await _IAdminProfile.GetProfileDetails((id != null ? (int)id : Convert.ToInt32(CV.UserID())));
            ViewBag.RegionComboBox = _combobox.RegionComboBox();
            //ViewBag.userrolecombobox = await _combobox.UserRoleComboBox();
            return View("../AdminProfile/Index", p);
        }
        #endregion

        #region EditPassword
        public async Task<IActionResult> EditPassword(string password)
        {
            if (await _IAdminProfile.EditPassword(password, Convert.ToInt32(CV.UserID())))
            {
                _notyf.Success("Password changed Successfully...");
            }
            else
            {
                _notyf.Error("Password not Changed...");
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region EditAdministratorInfo
        [HttpPost]
        public async Task<IActionResult> EditAdministratorInfo(ViewAdminProfileData _viewAdminProfile)
        {
            if (await _IAdminProfile.EditAdministratorInfo(_viewAdminProfile))
            {
                _notyf.Success("Information changed Successfully...");
            }
            else
            {
                _notyf.Error("Information not Changed...");
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region EditAdministratorInfo
        [HttpPost]
        public async Task<IActionResult> BillingInfoEdit(ViewAdminProfileData _viewAdminProfile)
        {
            if (await _IAdminProfile.BillingInfoEdit(_viewAdminProfile))
            {
                _notyf.Success("Information changed Successfully...");
            }
            else
            {
                _notyf.Error("Information not Changed...");
            }
            return RedirectToAction("Index");
        }
        #endregion
    }
}
