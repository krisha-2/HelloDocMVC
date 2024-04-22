using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HelloDocMVC.Repository.Repository.Interface;
using System.Collections;
using HelloDocMVC.Repository.Repository;

namespace HelloDocMVC.Controllers
{
    public class SchedulingController : Controller
    {

        private readonly HelloDocDbContext _context;
        private readonly IComboBox _comboBox;
        private readonly IScheduling _scheduling;
        public SchedulingController(HelloDocDbContext context, IComboBox comboBox, IScheduling scheduling)
        {
            _context = context;
            _comboBox = comboBox;
            _scheduling = scheduling;
        }
        public async Task<IActionResult> Index(int? region)
        {
            ViewBag.RegionComboBox = _comboBox.RegionComboBox();
            ViewBag.PhysiciansByRegion = new SelectList(Enumerable.Empty<SelectListItem>());
            SchedulingData modal = new SchedulingData();
            return View("../Scheduling/Index", modal);

        }
        public IActionResult GetPhysicianByRegion(int regionid)
        {
            var PhysiciansByRegion = _comboBox.ProviderbyRegion(regionid);

            return Json(PhysiciansByRegion);
        }
        public IActionResult LoadSchedulingPartial(string PartialName, string date, int regionid)
        {
            var currentDate = DateTime.Parse(date);
            List<Physician> physician = _context.PhysicianRegions.Include(u => u.Physician).Where(u => u.RegionId == regionid).Select(u => u.Physician).ToList();
            if (regionid == 0)
            {
                physician = _context.Physicians.ToList();
            }
            switch (PartialName)
            {
                case "_DayWise":
                    DayWiseScheduling day = new DayWiseScheduling
                    {
                        date = currentDate,
                        physicians = physician,
                        shiftdetails = _context.ShiftDetailRegions.Include(u => u.ShiftDetail).ThenInclude(u => u.Shift).Where(u => u.RegionId == regionid && u.IsDeleted == new BitArray(new[] { false })).Select(u => u.ShiftDetail).ToList()
                    };
                    if (regionid == 0)
                    {
                        day.shiftdetails = _context.ShiftDetails.Include(u => u.Shift).Where(u => u.IsDeleted == new BitArray(new[] { false })).ToList();
                    }
                    return PartialView("../Scheduling/_DayWise", day);

                case "_WeekWise":
                    WeekWiseScheduling week = new WeekWiseScheduling
                    {
                        date = currentDate,
                        physicians = physician,
                        shiftdetails = _context.ShiftDetailRegions.Include(u => u.ShiftDetail).ThenInclude(u => u.Shift).ThenInclude(u => u.Physician).Where(u => u.IsDeleted == new BitArray(new[] { false })).Where(u => u.RegionId == regionid).Select(u => u.ShiftDetail).ToList()
                    };
                    if (regionid == 0)
                    {
                        week.shiftdetails = _context.ShiftDetails.Include(u => u.Shift).ThenInclude(u => u.Physician).Where(u => u.IsDeleted == new BitArray(new[] { false })).ToList();
                    }
                    return PartialView("../Scheduling/_WeekWise", week);

                case "_MonthWise":
                    MonthWiseScheduling month = new MonthWiseScheduling
                    {
                        date = currentDate,
                        shiftdetails = _context.ShiftDetailRegions.Include(u => u.ShiftDetail).ThenInclude(u => u.Shift).ThenInclude(u => u.Physician).Where(u => u.IsDeleted == new BitArray(new[] { false })).Where(u => u.RegionId == regionid).Select(u => u.ShiftDetail).ToList()
                    };
                    if (regionid == 0)
                    {
                        month.shiftdetails = _context.ShiftDetails.Include(u => u.Shift).ThenInclude(u => u.Physician).Where(u => u.IsDeleted == new BitArray(new[] { false })).ToList();
                    }
                    return PartialView("../Scheduling/_MonthWise", month);

                default:
                    return PartialView("../Scheduling/_DayWise");
            }
        }
        public IActionResult LoadSchedulingPartialProivder(string date)
        {
            var currentDate = DateTime.Parse(date);
            MonthWiseScheduling month = new MonthWiseScheduling
            {
                date = currentDate,
                shiftdetails = _context.ShiftDetails.Include(u => u.Shift).Where(u => u.IsDeleted == new BitArray(new[] { false }) && u.Shift.PhysicianId == Int32.Parse(CV.UserID())).ToList()
            };
            return PartialView("../Scheduling/_MonthWise", month);
        }
        public IActionResult AddShift(SchedulingData model)
        {
            string adminId = CV.ID();
            var chk = Request.Form["repeatdays"].ToList();
            _scheduling.AddShift(model, chk, adminId);
            return RedirectToAction("Index");
        }
        public SchedulingData viewshift(int shiftdetailid)
        {
            SchedulingData modal = new SchedulingData();
            var shiftdetail = _context.ShiftDetails.FirstOrDefault(u => u.ShiftDetailId == shiftdetailid);

            if (shiftdetail != null)
            {
                _context.Entry(shiftdetail)
                    .Reference(s => s.Shift)
                    .Query()
                    .Include(s => s.Physician)
                    .Load();
            }
            modal.regionid = (int)shiftdetail.RegionId;
            modal.physicianname = shiftdetail.Shift.Physician.FirstName + " " + shiftdetail.Shift.Physician.LastName;
            modal.modaldate = shiftdetail.ShiftDate.ToString("yyyy-MM-dd");
            modal.starttime = shiftdetail.StartTime;
            modal.endtime = shiftdetail.EndTime;
            modal.shiftdetailid = shiftdetailid;
            return modal;
        }
        public IActionResult ViewShiftreturn(SchedulingData modal)
        {
            var shiftdetail = _context.ShiftDetails.FirstOrDefault(u => u.ShiftDetailId == modal.shiftdetailid);
            if (shiftdetail.Status == 0)
            {
                shiftdetail.Status = 1;
            }
            else
            {
                shiftdetail.Status = 0;
            }
            _context.ShiftDetails.Update(shiftdetail);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public void ViewShiftSave(SchedulingData modal)
        {
            _scheduling.ViewShiftSave(modal, CV.ID());
        }
        public IActionResult ViewShiftDelete(SchedulingData modal)
        {
            _scheduling.ViewShiftDelete(modal, CV.ID());

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> MDSOnCall(int? regionId)
        {
            ViewBag.RegionComboBox = _comboBox.RegionComboBox();
            List<ViewProvider> v = await _scheduling.PhysicianOnCall(regionId);
            if (regionId != null)
            {
                return Json(v);
            }
            return View("../Scheduling/MDsOnCall", v);
        }
        public IActionResult RequestedShift(int? regionId,SchedulingData sd)
        {
            ViewBag.RegionComboBox = _comboBox.RegionComboBox();
            SchedulingData v =  _scheduling.GetAllNotApprovedShift(regionId,sd);

            return View("../Scheduling/ReviewShift", v);
        }
        public async Task<IActionResult> _ApprovedShifts(string shiftids)
        {
            if (await _scheduling.UpdateStatusShift(shiftids, CV.ID()))
            {
                TempData["Status"] = "Approved Shifts Successfully..!";
            }


            return RedirectToAction("RequestedShift");
        }
        public async Task<IActionResult> _DeleteShifts(string shiftids)
        {
            if (await _scheduling.DeleteShift(shiftids, CV.ID()))
            {
                TempData["Status"] = "Delete Shifts Successfully..!";
            }

            return RedirectToAction("RequestedShift");
        }
    }
}