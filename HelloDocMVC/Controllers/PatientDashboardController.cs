using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace HallodocMVC.Controllers
{
    public class PatientDashboardController : Controller
    {
        private readonly HelloDocDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPatientDashboard _patientDashboard;
        public PatientDashboardController(HelloDocDbContext context, IHttpContextAccessor httpContextAccessor, IPatientDashboard patientDashboard)

        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _patientDashboard = patientDashboard;
        }
        public IActionResult Index()
        {

           int id = (int)_httpContextAccessor.HttpContext.Session.GetInt32("id");
           
           var Result = _patientDashboard.PatientDashboardList(id);

        
            return View(Result);

        }

//        public IActionResult Document(int? id)
//        {

//            List<Request> Request = _context.Requests.Where(r => r.RequestId == id).ToList();
//            ViewBag.requestinfo = Request;
//            List<RequestWiseFile> DocList = _context.RequestWiseFiles.Where(r => r.RequestId == id).ToList();
//            ViewBag.DocList = DocList;
//            return View("Document");
//        }

//        public IActionResult UploadDoc(int RequestId, IFormFile? UploadFile)
//        {
//            string UploadImage;
//            if (UploadFile != null)
//            {
//                string FilePath = "wwwroot\\Upload";
//                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
//                if (!Directory.Exists(path))
//                    Directory.CreateDirectory(path);
//                string fileNameWithPath = Path.Combine(path, UploadFile.FileName);
//                UploadImage = "~" + FilePath.Replace("wwwroot\\", "/") + "/" + UploadFile.FileName;
//                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
//                {
//                    UploadFile.CopyTo(stream)
//;
//                }
//                var requestwisefile = new RequestWiseFile
//                {
//                    RequestId = RequestId,
//                    FileName = UploadFile.FileName,
//                    CreatedDate = DateTime.Now,
//                };
//                _context.RequestWiseFiles.Add(requestwisefile);
//                _context.SaveChanges();
//            }

//            return RedirectToAction("Document", new { id = RequestId });
//        }





    }
}

