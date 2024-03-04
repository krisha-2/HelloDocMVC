using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Models;
using HelloDocMVC.Repository.Repository;
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
        public IActionResult Document(int RequestId)
        {
            var result = _patientDashboard.Document(RequestId);
            return View(result);
        }
        [HttpPost]
        public IActionResult Document(int RequestId, IFormFile? UploadFile)
        {
             _patientDashboard.UploadDoc(RequestId, UploadFile);
            return RedirectToAction("Document", new { id = RequestId });
        }
        public IActionResult PatientProfile()
        {
            var UsersProfile = _context.Users
                                .Where(r => Convert.ToString(r.AspNetUserId) == (CV.UserID()))
                                .Select(r => new UserProfile
                                {
                                    UserId = r.UserId,
                                    FirstName = r.FirstName,
                                    LastName = r.LastName,
                                    Mobile = r.Mobile,
                                    Email = r.Email,
                                    Street = r.Street,
                                    State = r.State,
                                    City = r.City,
                                    ZipCode = r.ZipCode,
                                    DOB = new DateTime((int)r.IntYear, Convert.ToInt32(r.StrMonth.Trim()), (int)r.IntDate),
                                })
                                .FirstOrDefault();
            return View(UsersProfile);
        }
        public async Task<IActionResult> Edit(UserProfile userprofile)
        {
            try
            {
                User userToUpdate = await _context.Users.FindAsync(userprofile.UserId);

                userToUpdate.FirstName = userprofile.FirstName;
                userToUpdate.LastName = userprofile.LastName;
                userToUpdate.Mobile = userprofile.Mobile;
                userToUpdate.Email = userprofile.Email;
                userToUpdate.State = userprofile.State;
                userToUpdate.Street = userprofile.Street;
                userToUpdate.City = userprofile.City;
                userToUpdate.ZipCode = userprofile.ZipCode;
                userToUpdate.IntDate = userprofile.DOB.Day;
                userToUpdate.IntYear = userprofile.DOB.Year;
                userToUpdate.StrMonth = userprofile.DOB.Month.ToString();
                userToUpdate.ModifiedBy = userprofile.CreatedBy;
                userToUpdate.ModifiedDate = DateTime.Now;
                _context.Update(userToUpdate);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(userprofile.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Index");
        }
        private bool UserExists(object id)
        {
            throw new NotImplementedException();
        }
    }
}

