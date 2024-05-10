using Microsoft.AspNetCore.Mvc;

namespace HelloDocMVC.Controllers
{
    public class InvoicingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
