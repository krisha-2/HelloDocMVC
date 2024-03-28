using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Repository.Repository.Interface;
using HelloDocMVC.Repository.Repository;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Models;

namespace HelloDocMVC.Controllers
{
    public class ProviderController : Controller
    {
        private readonly IProvider _IProvider;
        private readonly INotyfService _notyf;
        private readonly IComboBox _comboBox;
        private readonly ILogger<AdminController> _logger;
        private readonly EmailConfiguration _emailConfig;

        public ProviderController(ILogger<AdminController> logger,IProvider IProvider,INotyfService notyf, IComboBox comboBox,EmailConfiguration emailConfiguration)
        {
            _IProvider = IProvider;
            _notyf = notyf;
            _logger = logger;
            _comboBox = comboBox;
            _emailConfig = emailConfiguration;
        }
        #region Index
        public async Task<IActionResult> IndexAsync(int? region)
        {
            ViewBag.RegionComboBox = _comboBox.RegionComboBox();
            var v = await _IProvider.PhysicianAll();
            if (region == null)
            {
                v = await _IProvider.PhysicianAll();
            }
            else
            {
                v = await _IProvider.PhysicianByRegion(region);
            }
            return View("../Provider/Index", v);
        }
        #endregion

        #region ChangeNotificationPhysician
        public async Task<IActionResult> ChangeNotificationPhysician(string changedValues)
        {
            Dictionary<int, bool> changedValuesDict = JsonConvert.DeserializeObject<Dictionary<int, bool>>(changedValues);
            _IProvider.ChangeNotificationPhysician(changedValuesDict);
            return RedirectToAction("Index");
        }
        #endregion
        #region AddEdit_Profile
        public async Task<IActionResult> Edit(int? id)
        {

            //TempData["Status"] = TempData["Status"];
            ViewBag.RegionComboBox = _comboBox.RegionComboBox();
            ViewBag.UserRolecombobox = await _comboBox.UserRoleComboBox();
            if (id == null)
            {
                ViewData["PhysicianAccount"] = "Add";
            }
            else
            {
                ViewData["PhysicianAccount"] = "Edit";

                return View("../Provider/Edit");

            }
            return View("../Provider/Edit");
        }
        #endregion
        #region SendMessage
        public async Task<IActionResult> SendMessage(string? email, int? way, string? msg)
        {
            bool result;
            if (way == 1)
            {
                result = await _emailConfig.SendMail(email, "Check massage", "Hello " + msg);
            }
            else if (way == 2)
            {
                result = await _emailConfig.SendMail(email, "Check massage", "Hello " + msg);
            }
            else
            {
                result = await _emailConfig.SendMail(email, "Check massage", "Hello " + msg);
            }
            if (result)
            {
                _notyf.Success("Massage Send Successfully..!");
            }
            else
            {
                _notyf.Error("Massage Not Send..!");
            }
            return RedirectToAction("Index");
        }
        #endregion
        #region Physician_Add
        [HttpPost]
        public async Task<IActionResult> PhysicianAddEdit(ViewProvider physicians)
        {
            //TempData["Status"] = TempData["Status"];
            ViewBag.RegionComboBox = _comboBox.RegionComboBox();
            ViewBag.UserRolecombobox = await _comboBox.UserRoleComboBox();
            // bool b = physicians.Isagreementdoc[0];

            if (ModelState.IsValid)
            {
                await _IProvider.PhysicianAddEdit(physicians, CV.ID());
            }
            else
            {
                return View("../Provider/Edit", physicians);
            }

            return RedirectToAction("PhysicianAll");
        }
        #endregion
    }
}