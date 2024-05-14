using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Repository.Repository.Interface
{
    public interface IInvoicing
    {
        bool isFinalizeTimesheet(int PhysicianId, DateOnly StartDate);
        bool isApprovedTimesheet(int PhysicianId, DateOnly StartDate);
        List<TimeSheetDetail> PostTimesheetDetails(int PhysicianId, DateOnly StartDate, int AfterDays, string AdminId);
        ViewTimeSheet GetTimesheetDetails(List<TimeSheetDetail> td, List<ViewTimeSheetDetailReimbursementsdata> tr, int PhysicianId);
        bool PutTimesheetDetails(List<ViewTimeSheetDetails> tds, string AdminId);
        bool TimeSheetBillAddEdit(ViewTimeSheetDetailReimbursementsdata trb, string AdminId);
        Task<List<ViewTimeSheetDetailReimbursementsdata>> GetTimesheetBills(List<TimeSheetDetail> TimeSheetDetails);
        bool SetToFinalize(int timesheetid, string AdminId);
        bool TimeSheetBillRemove(ViewTimeSheetDetailReimbursementsdata trb, string AdminId);
        Task<bool> SetToApprove(ViewTimeSheet vts, string AdminId);
        List<ViewProvider> GetAllPhysicians();
        public List<TimeSheet> GetPendingTimesheet(int PhysicianId, DateOnly StartDate);
    }
}
