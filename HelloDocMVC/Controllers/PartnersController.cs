using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Repository.Repository;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HelloDocMVC.Controllers
{
    public class PartnersController : Controller
    {
        #region Constructor
        private readonly IPartners _IPartners;
        private readonly IProvider _IProvider;
        private readonly IAdminProfile _IMyProfileRepository;
        private readonly INotyfService _notyf;
        private readonly IComboBox _comboBox;
        private readonly ILogger<AdminController> _logger;
        private readonly EmailConfiguration _emailConfig;

        public PartnersController(ILogger<AdminController> logger, IPartners IPartnersRepository, INotyfService notyf, IComboBox combobox, EmailConfiguration emailConfiguration)
        {
            _IPartners = IPartnersRepository;
            _notyf = notyf;
            _logger = logger;
            _comboBox = combobox;
            _emailConfig = emailConfiguration;
        }
        #endregion
        public async Task<IActionResult> Index(int? regionId)
        {
            ViewBag.RegionComboBox = _comboBox.RegionComboBox();
            List<HealthProfessional> data = await _IPartners.GetPartnersByProfession(regionId);
            return View("../Partners/Index", data);
        }
    }
}
