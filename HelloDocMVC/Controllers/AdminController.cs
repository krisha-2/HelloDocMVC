using HelloDocMVC.Entity.Models;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HelloDocMVC.Entity;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Models;
using Microsoft.EntityFrameworkCore;
using static HelloDocMVC.Entity.Models.Constant;

namespace HelloDocMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminDashboard _IAdminDashboard;

        public AdminController(IAdminDashboard adminDashboard)
        {
            _IAdminDashboard = adminDashboard;
        }
        //public IActionResult Index()
        //{
        //    var Data = _IAdminDashboard.IndexData();
        //    return View(Data);
        //}
        public IActionResult Index()
        {
            CountStatusWiseRequestModel sm = _IAdminDashboard.IndexData();

            return View(sm);
        }
        public IActionResult ViewCase(int? id)
        {
            var Data = _IAdminDashboard.ViewCaseData(id);
            return View(Data);
        }
        public IActionResult ViewNotes()
        {
            //var Data = _IAdminDashboard.ViewCaseData(id);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _SearchResult(string Status)
        {
            if (Status == null)
            {
                Status = "1";
            }
            List<AdminDashboardList> contacts = _IAdminDashboard.GetRequests(Status);

            switch (Status)
            {
                case "1":
                    return PartialView("../Admin/_New", contacts);

                    break;
                case "2":

                    return PartialView("../Admin/_Pending", contacts);
                    break;
                case "4,5":

                    return PartialView("../Admin/_Active", contacts);
                    break;
                case "6":

                    return PartialView("../Admin/_Conclude", contacts);
                    break;
                case "3,7,8":

                    return PartialView("../Admin/_ToClose", contacts);
                    break;
                case "9":

                    return PartialView("../Admin/_UnPaid", contacts);
                    break;
             
            }


            return PartialView("../Admin/_New", contacts);
        }

    }
}
      
