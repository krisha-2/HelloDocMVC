using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Repository.Repository.Interface;
using HelloDocMVC.Repository.Repository;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Models;
using Twilio.TwiML.Messaging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HelloDocMVC.Controllers
{
    public class ProviderController : Controller
    {
        private readonly IProvider _IProvider;
        private readonly IComboBox _comboBox;
        private readonly ILogger<AdminController> _logger;
        private readonly EmailConfiguration _emailConfig;

        public ProviderController(ILogger<AdminController> logger, IProvider IProvider, IComboBox comboBox, EmailConfiguration emailConfiguration)
        {
            _IProvider = IProvider;
            _logger = logger;
            _comboBox = comboBox;
            _emailConfig = emailConfiguration;
        }
        public async Task<IActionResult> IndexAsync(int? region, ViewProvider vp)
        {
            ViewBag.RegionComboBox = _comboBox.RegionComboBox();
            var v = _IProvider.PhysicianAll(vp);
            if (region == null)
            {
                v = _IProvider.PhysicianAll(vp);
            }
            else
            {
                v = _IProvider.PhysicianByRegion(region, vp);
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
            bool result = false;
            bool sms = false;
            if (way == 1)
            {
                sms = _IProvider.SendMessage(msg);
            }
            else if (way == 2)
            {
                result = await _emailConfig.SendMail(email, "Check massage", "Hello " + msg);
            }
            else
            {
                result = await _emailConfig.SendMail(email, "Check massage", "Hello " + msg);
                sms = _IProvider.SendMessage(msg);
            }
            if (result)
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Email Sent Successfully";
            }
            if (sms)
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Message Sent Successfully";
            }
            return RedirectToAction("Index");

        }
        public async Task<IActionResult> PhysicianProfile(int? id)
        {

            //TempData["Status"] = TempData["Status"];
            ViewBag.RegionComboBox = _comboBox.RegionComboBox();
            ViewBag.PhysicianRoleComboBox = await _comboBox.PhysicianRoleComboBox();
            if (id == null)
            {
                ViewData["PhysicianAccount"] = "Add";
            }
            else
            {
                ViewData["PhysicianAccount"] = "Edit";
                ViewProvider v = await _IProvider.GetPhysicianById((int)id);
                return View("../Provider/Edit", v);

            }
            return View("../Provider/Edit");
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ViewProvider physicians)
        {
            ViewBag.RegionComboBox = _comboBox.RegionComboBox();
            ViewBag.PhysicianRoleComboBox = await _comboBox.PhysicianRoleComboBox();
            if (await _IProvider.PhysicianAddEdit(physicians, CV.ID()))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Physician added Successfully";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Physician not added Successfully";
            }
            return RedirectToAction("UserRole", "Access");
        }
        public async Task<IActionResult> EditAccountInfo(ViewProvider data)
        {
            string actionName = RouteData.Values["action"].ToString();
            string actionNameaq = ControllerContext.ActionDescriptor.ActionName; // Get the current action name
            if (await _IProvider.EditAccountInfo(data))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Account Information Changed Successfully";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Account Information not Changed Successfully";
            }
            return RedirectToAction("Edit", new { id = data.Physicianid });
        }
        public async Task<IActionResult> ResetPassAdmin(string password, int Physicianid)
        {
            if (await _IProvider.ChangePasswordAsync(password, Physicianid))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Password Information Changed Successfully";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Password Information not Changed Successfully";
            }
            return RedirectToAction("Edit", new { id = Physicianid });
        }
        public async Task<IActionResult> EditPhysicianInfo(ViewProvider data)
        {
            //ViewBag.RegionComboBox = _comboBox.RegionComboBox();
            //ViewBag.PhysicianRoleComboBox = await _comboBox.PhysicianRoleComboBox();
            if (await _IProvider.EditPhysicianInfo(data))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Administrator Information Changed Successfully";
                return RedirectToAction("PhysicianProfile", new { id = data.Physicianid });
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Administrator Information not Changed Successfully";
                return RedirectToAction("PhysicianProfile", new { id = data.Physicianid });
            }
        }
        public async Task<IActionResult> EditMailBillingInfo(ViewProvider data)
        {
            if (await _IProvider.EditMailBillingInfo(data, CV.ID()))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "mail and billing Information Changed Successfully";
                return RedirectToAction("Edit", new { id = data.Physicianid });
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "mail and billing Information not Changed Successfully";
                return RedirectToAction("Edit", new { id = data.Physicianid });
            }
        }
        public async Task<IActionResult> EditProviderProfile(ViewProvider data)
        {
            if (await _IProvider.EditProviderProfile(data, CV.ID()))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Profile Changed Successfully";
                return RedirectToAction("Edit", new { id = data.Physicianid });
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Profile not Changed Successfully";
                return RedirectToAction("Edit", new { id = data.Physicianid });
            }
        }
        public async Task<IActionResult> EditProviderOnbording(ViewProvider data)
        {
            if (await _IProvider.EditProviderOnbording(data, CV.ID()))
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Provider Onbording Changed Successfully";
                return RedirectToAction("Edit", new { id = data.Physicianid });
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Provider Onbording not Changed Successfully";
                return RedirectToAction("Edit", new { id = data.Physicianid });
            }
        }
        public async Task<IActionResult> DeletePhysician(int PhysicianID)
        {
            bool data = await _IProvider.DeletePhysician(PhysicianID, CV.ID());
            if (data)
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Physician deleted successfully";
                return RedirectToAction("Index");
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Physician not deleted successfully";
                return RedirectToAction("PhysicianAll");
            }
        }
        public async Task<IActionResult> Location()
        {
            ViewBag.Log = _IProvider.FindPhysicianLocation();
            return View();
        }
        public IActionResult PayRate(int id)
        {
            Payrate p = _IProvider.GetPayrate(id);
            return View("../Provider/PayRate", p);
        }
        [HttpPost]
        public IActionResult PayratePost(Payrate pr)
        {
            string id = CV.ID();
            if (_IProvider.PayratePost(pr, id))
            {
                //_notyf.Success("Payrate Information changed Successfully...");
            }
            else
            {
                //_notyf.Error("Payrate Information not Changed...");
            }
            return RedirectToAction("PayRate", new { id = pr.PhysicianId });
        }
    }
}