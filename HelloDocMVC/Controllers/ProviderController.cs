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
        public async Task<IActionResult> ChangeNotificationPhysician(string changedValues)
        {
            Dictionary<int, bool> changedValuesDict = JsonConvert.DeserializeObject<Dictionary<int, bool>>(changedValues);
            _IProvider.ChangeNotificationPhysician(changedValuesDict);
            return RedirectToAction("Index");
        }
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
        public async Task<IActionResult> PhysicianProfile(int? id)
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
                ViewProvider v = await _IProvider.GetPhysicianById((int)id);
                return View("../Provider/Edit",v);

            }
            return View("../Provider/Edit");
        }
        [HttpPost]
        public async Task<IActionResult> PhysicianAdd(ViewProvider physicians)
        {
            ViewBag.RegionComboBox = _comboBox.RegionComboBox();
            ViewBag.UserRolecombobox = await _comboBox.UserRoleComboBox();
            // bool b = physicians.Isagreementdoc[0];

            /*if (ModelState.IsValid)
            {*/

            /*}
            else
            {
                return View("../Admin/Providers/AddEditProvider", physicians);
            }*/
            if (await _IProvider.PhysicianAddEdit(physicians, CV.ID()))
			{
				_notyf.Success("Physician added Successfully..!");
			}
			else
			{
				_notyf.Error("Physician not added Successfully..!");
			}
			return RedirectToAction("Index");
        }
        public async Task<IActionResult> EditAccountInfo(ViewProvider data)
        {
            string actionName = RouteData.Values["action"].ToString();
            string actionNameaq = ControllerContext.ActionDescriptor.ActionName; // Get the current action name
            if (await _IProvider.EditAccountInfo(data))
            {
                _notyf.Success("Account Information Changed Successfully..!");
            }
            else
            {
                _notyf.Error("Account Information not Changed Successfully..!");
            }
            return RedirectToAction("PhysicianProfile", new { id = data.Physicianid });
        }
        public async Task<IActionResult> ResetPassAdmin(string password, int Physicianid)
        {
            if (await _IProvider.ChangePasswordAsync(password, Physicianid))
            {
                _notyf.Success("Password Information Changed Successfully..!");
            }
            else
            {
                _notyf.Error("Password not Changed properly Successfully..!");
            }
            return RedirectToAction("PhysicianProfile", new { id = Physicianid });
        }
        public async Task<IActionResult> EditPhysicianInfo(ViewProvider data)
        {
            if (await _IProvider.EditPhysicianInfo(data))
            {
                _notyf.Success("Administrator Information Changed Successfully..!");
                return RedirectToAction("PhysicianProfile", new { id = data.Physicianid });
            }
            else
            {
                _notyf.Error("Administrator Information not Changed Successfully..!");
                return RedirectToAction("PhysicianProfile", new { id = data.Physicianid });
            }
        }
        public async Task<IActionResult> EditMailBillingInfo(ViewProvider data)
        {
            if (await _IProvider.EditMailBillingInfo(data, CV.ID()))
            {
                _notyf.Success("mail and billing Information Changed Successfully...");
                return RedirectToAction("PhysicianProfile", new { id = data.Physicianid });
            }
            else
            {
                _notyf.Error("mail and billing Information not Changed Successfully...");
                return RedirectToAction("PhysicianProfile", new { id = data.Physicianid });
            }
        }
        public async Task<IActionResult> EditProviderProfile(ViewProvider data)
        {
            if (await _IProvider.EditProviderProfile(data, CV.ID()))
            {
                _notyf.Success("Profile Changed Successfully...");
                return RedirectToAction("PhysicianProfile", new { id = data.Physicianid });
            }
            else
            {
                _notyf.Error("Profile not Changed Successfully...");
                return RedirectToAction("PhysicianProfile", new { id = data.Physicianid });
            }
        }
        public async Task<IActionResult> EditProviderOnbording(ViewProvider data)
        {
            if (await _IProvider.EditProviderOnbording(data, CV.ID()))
            {
                _notyf.Success("Provider Onbording Changed Successfully...");
                return RedirectToAction("PhysicianProfile", new { id = data.Physicianid });
            }
            else
            {
                _notyf.Error("Provider Onbording not Changed Successfully...");
                return RedirectToAction("PhysicianProfile", new { id = data.Physicianid });
            }
        }
        public async Task<IActionResult> DeletePhysician(int PhysicianID)
        {
            bool data = await _IProvider.DeletePhysician(PhysicianID, CV.ID());
            if (data)
            {
                _notyf.Success("Physician deleted successfully...");
                return RedirectToAction("Index");
            }
            else
            {
                _notyf.Success("Physician not deleted successfully...");
                return RedirectToAction("PhysicianAll");
            }
        }
        public async Task<IActionResult> Location()
        {
            ViewBag.Log = _IProvider.FindPhysicianLocation();
            return View();
        }
    }
}