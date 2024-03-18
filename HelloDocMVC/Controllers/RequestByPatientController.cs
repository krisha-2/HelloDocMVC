//using HelloDoc.DataContext;
//using Microsoft.AspNetCore.Mvc;
//using HelloDoc.DataModels;
//using HelloDoc.Models;
//using System.Collections;
//namespace HelloDoc.Controllers
//{
//    public class RequestByPatientController : Controller
//    {
//        private readonly HelloDocDbContext _context;
//        public RequestByPatientController(HelloDocDbContext context)
//        {
//            _context = context;
//        }


//        public IActionResult Me()
//        {
//            var ViewPatientCreateRequest = _context.Users
//                               .Where(r => Convert.ToString(r.AspNetUserId) == CV.UserID())
//                               .Select(r => new Patient
//                               {
//                                   FirstName = r.FirstName,
//                                   LastName = r.LastName,
//                                   Email = r.Email,
//                                   Mobile = r.Mobile,
//                                   DOB = new DateTime((int)r.IntYear, Convert.ToInt32(r.StrMonth.Trim()), (int)r.IntDate),
//                               })
//                               .FirstOrDefault();
//            return View(ViewPatientCreateRequest);
//        }

//        public async Task<IActionResult> MeData(Models.Patient viewdata)
//        {
//            AspNetUser A = new AspNetUser();
//            User U = new User();
//            Request R = new Request();
//            RequestClient RClient = new RequestClient();
//            var isexist = _context.Users.FirstOrDefault(x => x.Email == viewdata.Email);

//            AspNetUser
//                if (isexist == null)
//            {
//                Guid g = Guid.NewGuid();
//                A.Id = g.ToString();
//                A.UserName = viewdata.FirstName + " " + viewdata.LastName;
//                A.Email = viewdata.Email;
//                A.PhoneNumber = viewdata.PhoneNumber;
//                A.CreatedDate = DateTime.Now;
//                _context.Add(A);
//                await _context.SaveChangesAsync();
//                User
//                    U.AspNetUserId = A.Id;
//                U.FirstName = viewdata.FirstName;
//                U.LastName = viewdata.LastName;
//                U.CreatedBy = viewdata.FirstName;
//                U.Email = viewdata.Email;
//                U.Mobile = viewdata.Mobile;
//                U.Street = viewdata.Street;
//                U.City = viewdata.City;
//                U.State = viewdata.State;
//                U.ZipCode = viewdata.ZipCode;
//                U.Status = 5;
//                U.StrMonth = (viewdata.DOB.Month).ToString();
//                U.IntDate = viewdata.DOB.Day;
//                U.IntYear = viewdata.DOB.Year;
//                U.CreatedDate = DateTime.Now;
//                _context.Add(U);
//                await _context.SaveChangesAsync();
//            }
//            Request
//                R.RequestTypeId = 2;
//            if (isexist == null)
//            {
//                R.UserId = U.UserId;
//            }
//            else
//            {
//                R.UserId = isexist.UserId;
//            }
//            R.RFirstName = viewdata.FirstName;
//            R.RLastName = viewdata.LastName;
//            R.PhoneNumber = viewdata.PhoneNumber;
//            R.Email = viewdata.Email;
//            R.Status = 5;
//            R.RelationName = viewdata.RelationName;
//            R.ConfirmationNumber = R.PhoneNumber;
//            R.IsUrgentEmailSent = new BitArray(1);
//            R.CreatedDate = DateTime.Now;
//            _context.Add(R);
//            await _context.SaveChangesAsync();
//            RequestClient
//                RClient.RequestId = R.RequestId;
//            RClient.RcFirstName = viewdata.FirstName;
//            RClient.StrMonth = (viewdata.DOB.Month).ToString();
//            RClient.IntDate = viewdata.DOB.Day;
//            RClient.IntYear = viewdata.DOB.Year;
//            RClient.Address = viewdata.Street;
//            RClient.City = viewdata.City;
//            RClient.State = viewdata.State;
//            RClient.Notes = viewdata.Symptoms;
//            RClient.ZipCode = viewdata.ZipCode;
//            RClient.RcLastName = viewdata.LastName;
//            RClient.Email = viewdata.Email;
//            RClient.PhoneNumber = viewdata.PhoneNumber;
//            RClient.Address = viewdata.Street + "," + viewdata.City + "," + viewdata.State;
//            _context.Add(RClient);
//            await _context.SaveChangesAsync();

//            if (viewdata.UploadFile != null)
//            {
//                string FilePath = "wwwroot\\Uploads";
//                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

//                if (!Directory.Exists(path))
//                {
//                    Directory.CreateDirectory(path);
//                }
//                string fileNameWithPath = Path.Combine(path, viewdata.UploadFile.FileName);
//                viewdata.UploadImage = "~" + FilePath.Replace("wwwroot\\", "/") + "/" + viewdata.UploadFile.FileName;

//                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
//                {
//                    viewdata.UploadFile.CopyTo(stream);
//                }

//                var RequestWiseFile = new RequestWiseFile
//                {
//                    RequestId = R.RequestId,
//                    FileName = viewdata.UploadFile.FileName,
//                    CreatedDate = DateTime.Now,
//                };
//                _context.RequestWiseFiles.Add(RequestWiseFile);
//                _context.SaveChanges();
//            }


//            return RedirectToAction("Index", "Dashboard");
//        }



//        public IActionResult SomeoneElse()
//        {
//            return View();
//        }
//    }

//}
