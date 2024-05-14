using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Repository.Repository
{
    public class Invoicing : IInvoicing
    {
        #region Constructor
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly EmailConfiguration _emailConfig;
        private readonly HelloDocDbContext _context;
        public Invoicing(HelloDocDbContext context, EmailConfiguration emailConfig, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _emailConfig = emailConfig;
            this.httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Timesheet_Approved_Or_Not
        public bool isApprovedTimesheet(int PhysicianId, DateOnly StartDate)
        {
            var data = _context.TimeSheets.Where(e => e.PhysicianId == PhysicianId && e.StartDate == StartDate).FirstOrDefault();
            if (data.IsApproved == false)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Timesheet_Finalize_Or_Not
        public bool isFinalizeTimesheet(int PhysicianId, DateOnly StartDate)
        {
            var data = _context.TimeSheets.Where(e => e.PhysicianId == PhysicianId && e.StartDate == StartDate).FirstOrDefault();
            if (data == null)
            {
                return false;
            }
            else if (data.IsFinalize == false)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Set_To_Sheet_Finalize
        public bool SetToFinalize(int timesheetid, string AdminId)
        {
            try
            {
                var data = _context.TimeSheets.Where(e => e.TimeSheetId == timesheetid).FirstOrDefault();
                if (data != null)
                {
                    data.IsFinalize = true;
                    _context.TimeSheets.Update(data);
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }
        #endregion

        #region Set_To_Sheet_Approve
        public async Task<bool> SetToApprove(ViewTimeSheet vts, string AdminId)
        {
            try
            {
                var data = _context.TimeSheets.Where(e => e.TimeSheetId == vts.TimesheetId).FirstOrDefault();
                if (data != null)
                {
                    data.IsApproved = true;
                    data.BonusAmount = vts.Bonus;
                    data.AdminNotes = vts.AdminNotes;
                    _context.TimeSheets.Update(data);
                    _context.SaveChanges();
                }
                var d = httpContextAccessor.HttpContext.Request.Host;
                //var res = _context.Requestclients.FirstOrDefault(e => e.Requestid == v.RequestID);
                string emailContent = @"
                                <!DOCTYPE html>
                                <html lang=""en"">
                                <head>
                                 <meta charset=""UTF-8"">
                                 <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                 <title>Provider</title>
                                </head>
                                <body>
                                 <div style=""background-color: #f5f5f5; padding: 20px;"">
                                 <h2>Welcome to Our Healthcare Platform!</h2>
                                <p>Dear Provider ,</p>
                                <ol>
                                    <li>Your TimeSheet Startwith" + data.StartDate + @""" and End With " + data.EndDate + @""" Is Approve</li>
                                </ol>
                                <p>If you have any questions or need further assistance, please don't hesitate to contact us.</p>
                                <p>Thank you,</p>
                                <p>The Healthcare Team</p>
                                </div>
                                </body>
                                </html>
                                ";
                _emailConfig.SendMail("dasete8625@haislot.com", "New Order arrived", emailContent);
                EmailLog log = new()
                {
                    EmailTemplate = emailContent,
                    SubjectName = "New Order arrived",
                    EmailId = "dasete8625@haislot.com",
                    CreateDate = DateTime.Now,
                    SentDate = DateTime.Now,
                    PhysicianId = data.PhysicianId,
                    Action = 12,
                    IsEmailSent = new BitArray(new[] { true }),
                    SentTries = 1,
                    RoleId = 3,
                };
                _context.EmailLogs.Add(log);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Timesheet_Add
        public List<TimeSheetDetail> PostTimesheetDetails(int PhysicianId, DateOnly StartDate, int AfterDays, string AdminId)
        {
            var Timesheet = new TimeSheet();
            var data = _context.TimeSheets.Where(e => e.PhysicianId == PhysicianId && e.StartDate == StartDate).FirstOrDefault();
            if (data == null && AfterDays != 0)
            {
                DateOnly EndDate = StartDate.AddDays(AfterDays - 1);
                Timesheet.StartDate = StartDate;
                Timesheet.PhysicianId = PhysicianId;
                Timesheet.IsFinalize = false;
                Timesheet.EndDate = EndDate;
                Timesheet.CreatedDate = DateTime.Now;
                Timesheet.CreatedBy = AdminId;
                _context.TimeSheets.Add(Timesheet);
                _context.SaveChanges();

                for (DateOnly i = StartDate; i <= EndDate; i = i.AddDays(1))
                {
                    var Timesheetdetail = new TimeSheetDetail
                    {
                        TimeSheetId = Timesheet.TimeSheetId,
                        TimeSheetDate = i,
                        NumberofHousecall = 0,
                        NumberofPhonecall = 0,
                        TotalHours = 0
                    };
                    _context.TimeSheetDetails.Add(Timesheetdetail);
                    _context.SaveChanges();
                }
                return _context.TimeSheetDetails.Where(e => e.TimeSheetId == Timesheet.TimeSheetId).OrderBy(r => r.TimeSheetDate).ToList();
            }
            else if (data == null && AfterDays == 0)
            {
                return null;
            }
            else
            {
                return _context.TimeSheetDetails.Where(e => e.TimeSheetId == data.TimeSheetId).OrderBy(r => r.TimeSheetDate).ToList();
            }

        }
        #endregion

        #region Timesheet_Edit
        public bool PutTimesheetDetails(List<ViewTimeSheetDetails> tds, string AdminId)
        {
            try
            {
                foreach (var item in tds)
                {
                    var td = _context.TimeSheetDetails.Where(r => r.TimeSheetDetailId == item.TimeSheetDetailId).FirstOrDefault();
                    td.TotalHours = item.TotalHours;
                    td.NumberofHousecall = item.NumberofHousecall;
                    td.NumberofPhonecall = item.NumberofPhonecall;
                    td.IsWeekend = item.IsWeekend;
                    td.ModifiedBy = AdminId;
                    td.ModifiedDate = DateTime.Now;
                    _context.TimeSheetDetails.Update(td);
                    _context.SaveChanges();
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        #endregion

        #region Timesheet_Get
        public ViewTimeSheet GetTimesheetDetails(List<TimeSheetDetail> td, List<ViewTimeSheetDetailReimbursementsdata> tr, int PhysicianId)
        {
            try
            {
                var TimeSheet = new ViewTimeSheet();

                TimeSheet.ViewTimesheetDetails = td.Select(e => new ViewTimeSheetDetails
                {
                    IsWeekend = e.IsWeekend == null || e.IsWeekend == false ? false : true,
                    ModifiedBy = e.ModifiedBy,
                    ModifiedDate = e.ModifiedDate,
                    NumberofHousecall = e.NumberofHousecall,
                    NumberofPhonecall = e.NumberofPhonecall,
                    OnCallHours = FindOnCallProvider(PhysicianId, e.TimeSheetDate),
                    TimeSheetDate = e.TimeSheetDate,
                    TimeSheetDetailId = e.TimeSheetDetailId,
                    TotalHours = e.TotalHours,
                    TimeSheetId = e.TimeSheetId
                }).OrderBy(r => r.TimeSheetDate).ToList();
                if (tr != null)
                {
                    TimeSheet.ViewTimesheetDetailReimbursements = tr.Select(e => new ViewTimeSheetDetailReimbursementsdata
                    {
                        Amount = e.Amount,
                        TimeSheetDetailReimbursementId = e.TimeSheetDetailReimbursementId,
                        IsDeleted = e.IsDeleted,
                        ItemName = e.ItemName,
                        Bill = e.Bill,
                        CreatedDate = e.CreatedDate,
                        TimeSheetDate = e.TimeSheetDate,
                        TimesheetId = _context.TimeSheetDetails.Where(r => r.TimeSheetDetailId == e.TimeSheetDetailId).FirstOrDefault().TimeSheetId,
                        ModifiedBy = e.ModifiedBy,
                        TimeSheetDetailId = e.TimeSheetDetailId,
                    }).OrderBy(r => r.TimeSheetDetailId).ToList();
                }
                else
                {
                    TimeSheet.ViewTimesheetDetailReimbursements = new List<ViewTimeSheetDetailReimbursementsdata> { };
                }
                TimeSheet.PayrateWithProvider = _context.PhysicianPayrates.Where(r => r.PhysicianId == PhysicianId).ToList();
                if (td.Count > 0)
                {
                    TimeSheet.TimesheetId = TimeSheet.ViewTimesheetDetails[0].TimeSheetId;
                }
                TimeSheet.PhysicianId = PhysicianId;
                return TimeSheet;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        #endregion

        #region FindOnCallProvider
        public int FindOnCallProvider(int PhysicianId, DateOnly Timesheetdate)
        {
            int i = 0;
            var s = _context.Shifts.Where(r => r.PhysicianId == PhysicianId).ToList();
            foreach (var item in s)
            {
                i += _context.ShiftDetails.Where(r => r.ShiftId == item.ShiftId && DateOnly.FromDateTime(r.ShiftDate) == Timesheetdate).Count();
            }
            return i;
        }
        #endregion

        #region Timesheet_Bill_Get
        public async Task<List<ViewTimeSheetDetailReimbursementsdata>> GetTimesheetBills(List<TimeSheetDetail> TimeSheetDetails)
        {
            try
            {
                List<ViewTimeSheetDetailReimbursementsdata> TimeSheetBills = await (
                    from timesheetdoc in _context.TimeSheetDetailReimbursements
                    join timesheetdetail in _context.TimeSheetDetails // Join with the input list
                    on timesheetdoc.TimeSheetDetailId equals timesheetdetail.TimeSheetDetailId
                    where TimeSheetDetails.Contains(timesheetdoc.TimeSheetDetail) && !(timesheetdoc.IsDeleted ?? false)// Assuming IsDeleted is a property in Timesheetdetailreimbursements table
                    select new ViewTimeSheetDetailReimbursementsdata
                    {
                        TimeSheetDetailReimbursementId = timesheetdoc.TimeSheetDetailReimbursementId,
                        TimeSheetDetailId = timesheetdoc.TimeSheetDetailId,
                        ItemName = timesheetdoc.ItemName,
                        Amount = timesheetdoc.Amount,
                        TimeSheetDate = timesheetdetail.TimeSheetDate,
                        Bill = timesheetdoc.Bill,
                    }).ToListAsync();
                return TimeSheetBills;
            }
            catch (Exception e)
            {
                // Handle exceptions appropriately, logging or rethrowing as needed
                return null;
            }
        }
        #endregion

        #region TimeSheet_Bill_AddEdit
        public bool TimeSheetBillAddEdit(ViewTimeSheetDetailReimbursementsdata trb, string AdminId)
        {
            TimeSheetDetail data = _context.TimeSheetDetails.Where(e => e.TimeSheetDetailId == trb.TimeSheetDetailId).FirstOrDefault();
            if (data != null && trb.TimeSheetDetailReimbursementId == null)
            {
                TimeSheetDetailReimbursement timesheetdetailreimbursement = new TimeSheetDetailReimbursement();
                timesheetdetailreimbursement.TimeSheetDetailId = trb.TimeSheetDetailId;
                timesheetdetailreimbursement.Amount = (int)trb.Amount;
                timesheetdetailreimbursement.Bill = FileSave.UploadTimesheetDoc(trb.BillFile, data.TimeSheetId);
                timesheetdetailreimbursement.ItemName = trb.ItemName;
                timesheetdetailreimbursement.CreatedDate = DateTime.Now;
                timesheetdetailreimbursement.CreatedBy = AdminId;
                timesheetdetailreimbursement.IsDeleted = false;
                _context.TimeSheetDetailReimbursements.Add(timesheetdetailreimbursement);
                _context.SaveChanges();
                return true;
            }
            else if (data != null && trb.TimeSheetDetailReimbursementId != null)
            {
                TimeSheetDetailReimbursement timesheetdetailreimbursement = _context.TimeSheetDetailReimbursements.Where(r => r.TimeSheetDetailReimbursementId == trb.TimeSheetDetailReimbursementId).FirstOrDefault(); ;
                timesheetdetailreimbursement.Amount = (int)trb.Amount;

                timesheetdetailreimbursement.ItemName = trb.ItemName;
                timesheetdetailreimbursement.ModifiedDate = DateTime.Now;
                timesheetdetailreimbursement.ModifiedBy = AdminId;
                _context.TimeSheetDetailReimbursements.Update(timesheetdetailreimbursement);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion

        #region TimeSheetBill_Delete
        public bool TimeSheetBillRemove(ViewTimeSheetDetailReimbursementsdata trb, string AdminId)
        {
            TimeSheetDetailReimbursement data = _context.TimeSheetDetailReimbursements.Where(e => e.TimeSheetDetailReimbursementId == trb.TimeSheetDetailReimbursementId).FirstOrDefault();
            if (data != null)
            {
                data.ModifiedDate = DateTime.Now;
                data.ModifiedBy = AdminId;
                data.IsDeleted = true;
                _context.TimeSheetDetailReimbursements.Update(data);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        public List<ViewProvider> GetAllPhysicians()
        {
            var result = new List<ViewProvider>();
            result = (from py in _context.Physicians
                      select new ViewProvider
                      {
                          UserName = py.FirstName + " " + py.LastName,
                          Physicianid = py.PhysicianId,
                      }).ToList();
            return result;
        }
        public List<TimeSheet> GetPendingTimesheet(int PhysicianId, DateOnly StartDate)
        {
            var result = new List<TimeSheet>();
            result = (from timesheet in _context.TimeSheets
                      where (timesheet.IsApproved == false && timesheet.PhysicianId == PhysicianId && timesheet.StartDate == StartDate)
                      select timesheet).ToList();
            return result;
        }
    }
}

