﻿using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Repository.Repository;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HelloDocMVC.Controllers
{
    public class PartnersController : Controller
    {
        private readonly HelloDocDbContext _context;

        private readonly IPartners _IPartners;
        private readonly IProvider _IProvider;
        private readonly IAdminProfile _IMyProfileRepository;
        private readonly IComboBox _comboBox;
        private readonly ILogger<AdminController> _logger;
        private readonly EmailConfiguration _emailConfig;
        public PartnersController(ILogger<AdminController> logger, IPartners IPartnersRepository, IComboBox combobox, EmailConfiguration emailConfiguration, HelloDocDbContext context)
        {
            _context = context;
            _IPartners = IPartnersRepository;
            _logger = logger;
            _comboBox = combobox;
            _emailConfig = emailConfiguration;
        }
        public IActionResult Index(string searchValue, int Profession, PartnersData pd)
        {
            ViewBag.Profession = _context.HealthProfessionalTypes.ToList();
            PartnersData data = _IPartners.GetPartnersByProfession(searchValue, Profession, pd);
            return View("../Partners/Index", data);
        }
        public IActionResult AddEditBusiness(int VendorId)
        {
            if (VendorId == 0)
            {
                ViewData["Heading"] = "Add";
            }
            else
            {
                ViewData["Heading"] = "Update";
            }
            ViewBag.Professions = _context.HealthProfessionalTypes.ToList();
            var result = _IPartners.EditPartners(VendorId);
            return View("AddEditBusiness", result);
        }
        public IActionResult EditPartnersData(HealthProfessional hp)
        {
            var result = _IPartners.EditPartnersData(hp);
            if (result == true)
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Data edited Successfully";
            }
            else
            {
                // Error notification
                TempData["SweetAlertType"] = "error";
                TempData["SweetAlertMessage"] = "Data not Changed";
            }
            return RedirectToAction("Index");
        }
        public IActionResult DeleteBusiness(int VendorId)
        {
            bool res = _IPartners.DeleteBusiness(VendorId);
            if (res == true)
            {
                // Success notification
                TempData["SweetAlertType"] = "success";
                TempData["SweetAlertMessage"] = "Business Deleted Successfully";
            }
            return RedirectToAction("Index");
        }
    }
}
