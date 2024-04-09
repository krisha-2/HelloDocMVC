﻿using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocMVC.Entity.DataContext;
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
        private readonly HelloDocDbContext _context;

        private readonly IPartners _IPartners;
        private readonly IProvider _IProvider;
        private readonly IAdminProfile _IMyProfileRepository;
        private readonly INotyfService _notyf;
        private readonly IComboBox _comboBox;
        private readonly ILogger<AdminController> _logger;
        private readonly EmailConfiguration _emailConfig;
        public PartnersController(ILogger<AdminController> logger, IPartners IPartnersRepository, INotyfService notyf, IComboBox combobox, EmailConfiguration emailConfiguration, HelloDocDbContext context)
        {
            _context = context;
            _IPartners = IPartnersRepository;
            _notyf = notyf;
            _logger = logger;
            _comboBox = combobox;
            _emailConfig = emailConfiguration;
        }
        #endregion
        public IActionResult Index(string searchValue, int Profession)
        {
            ViewBag.Profession = _context.HealthProfessionalTypes.ToList();
            List<PartnersData> data = _IPartners.GetPartnersByProfession(searchValue, Profession);
            return View("../Partners/Index", data);
        }
        //public IActionResult AddEditBusiness(int VendorId)
        //{
        //    ViewBag.Profession = _context.HealthProfessionalTypes.ToList();

        //    //if (VendorId == 0)
        //    //{
        //    //    ViewData["Heading"] = "Add";
        //    //    return View("../Partners/AddEditBusiness");
        //    //}
        //    //else
        //    //{
        //    //    ViewData["Heading"] = "Update";
        //    //    var res = _IPartners.EditPartners(VendorId);
        //    //    return View("../Partners/AddEditBusiness", res);
        //    //}
        //}
        //[HttpPost]
        //public IActionResult AddEditBusiness(HealthProfessional obj)
        //{
        //    var res = _IPartners.AddBusiness(obj);

        //    if (res)
        //    {
        //        _notyf.Success("Data Added Successfully");
        //    }
        //    return RedirectToAction("Index");

        //}
        //public IActionResult DeleteBusiness(int VendorId)
        //{
        //    var res = _IPartners.DeleteBusiness(VendorId);

        //    if (res)
        //    {
        //        _notyf.Success("Vendor Deleted Successfully");
        //    }
        //    else
        //    {
        //        _notyf.Error("Vendor not deleted");
        //    }
        //    return RedirectToAction("Index");
        //}
    }
}
