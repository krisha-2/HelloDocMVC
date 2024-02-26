using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HelloDocMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminDashboard _IAdminDashboard;

        public AdminController(IAdminDashboard adminDashboard)
        {
            _IAdminDashboard = adminDashboard;
        }
        public IActionResult Index()
        {
            var Data = _IAdminDashboard.IndexData();
            return View(Data);
        }
    }
}
