using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Repository.Repository.Interface;
using HelloDocMVC.Repository.Repository;
using HelloDocMVC.Entity.Models;

namespace HelloDocMVC.Controllers
{
    public class ProviderController : Controller
    {
        private readonly IProvider _IProvider;
        private readonly INotyfService _notyf;
        private readonly IComboBox _comboBox;
        private readonly ILogger<AdminController> _logger;
        private readonly EmailConfiguration _emailConfiguration;

        public ProviderController(ILogger<AdminController> logger,IProvider IProvider,INotyfService notyf, IComboBox comboBox,EmailConfiguration emailConfiguration)
        {
            _IProvider = IProvider;
            _notyf = notyf;
            _logger = logger;
            _comboBox = comboBox;
            _emailConfiguration = emailConfiguration;
        }
        #region Index
        public async Task<IActionResult> IndexAsync(int? region)
        {
            ViewBag.RegionComboBox = _comboBox.RegionComboBox();
            var v = await _IProvider.PhysicianAll();
            //if (region == null)
            //{
                v = await _IProvider.PhysicianAll();
            //}
            //else
            //{
            //    v = await _IProvider.PhysicianByRegion(region);
            //}
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
        public IActionResult Edit()
        {
            return View();
        }
    }
}