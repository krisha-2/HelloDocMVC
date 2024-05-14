using HelloDocMVC.Entity.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Entity.Models
{
    public class ViewTimeSheet
    {
        public List<ViewTimeSheetDetails>? ViewTimesheetDetails { get; set; }

        public List<ViewTimeSheetDetailReimbursementsdata>? ViewTimesheetDetailReimbursements { get; set; }

        public List<PhysicianPayrate>? PayrateWithProvider { get; set; }

        public int TimesheetId { get; set; }

        public string? Bonus { get; set; }

        public string? AdminNotes { get; set; }

        public int PhysicianId { get; set; }
    }
    public class ViewTimeSheetDetails
    {
        public int TimeSheetDetailId { get; set; }

        public int TimeSheetId { get; set; }

        public DateOnly TimeSheetDate { get; set; }

        public int? OnCallHours { get; set; }

        public decimal? TotalHours { get; set; }

        public bool IsWeekend { get; set; }

        public int? NumberofHousecall { get; set; }

        public int? NumberofPhonecall { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
    public class ViewTimeSheetDetailReimbursementsdata
    {
        public int? TimeSheetDetailReimbursementId { get; set; } = null!;

        public int TimeSheetDetailId { get; set; }

        public int TimesheetId { get; set; }

        public string ItemName { get; set; } = null!;

        public int? Amount { get; set; } = null!;

        public DateOnly TimeSheetDate { get; set; }

        public string Bill { get; set; } = null!;

        public IFormFile? BillFile { get; set; }

        public bool? IsDeleted { get; set; }

        public string CreatedBy { get; set; } = null!;

        public DateTime CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }
    }
}
