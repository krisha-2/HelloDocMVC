using Assignment.Entity.Models;
using Assignment.Models;
using Assignment.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Assignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;

        public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository)
        {
            _logger = logger;
            _homeRepository = homeRepository;
        }

        public IActionResult Index()
        {
            var result = _homeRepository.ViewTableRequest();
            return View(result);
        }
        public IActionResult EditStudent(int Id)
        {
            var result = _homeRepository.StudentData(Id);
            return View("../Home/EditStudent", result);
        }

        public IActionResult DeleteBusiness(int Id)
        {
            bool res = _homeRepository.DeleteBusiness(Id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditData(ViewStudents viewdata)
        {
            var result = _homeRepository.EditData(viewdata);
            return RedirectToAction("Index", result);
        }
        public IActionResult AddStudent(ViewStudents vs)
        {
            var result = _homeRepository.AddStudent(vs);
            return RedirectToAction("Index",result);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}