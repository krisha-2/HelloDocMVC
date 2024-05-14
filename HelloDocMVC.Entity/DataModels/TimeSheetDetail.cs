using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HelloDocMVC.Entity.DataModels;

[Table("TimeSheetDetail")]
public partial class TimeSheetDetail
{
    [Key]
    public int TimeSheetDetailId { get; set; }

    public int TimeSheetId { get; set; }

    public DateOnly TimeSheetDate { get; set; }

    public decimal? TotalHours { get; set; }

    public bool? IsWeekend { get; set; }

    public int? NumberofHousecall { get; set; }

    public int? NumberofPhonecall { get; set; }

    [StringLength(128)]
    public string? ModifiedBy { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("TimeSheetDetails")]
    public virtual AspNetUser? ModifiedByNavigation { get; set; }

    [ForeignKey("TimeSheetId")]
    [InverseProperty("TimeSheetDetails")]
    public virtual TimeSheet TimeSheet { get; set; } = null!;

    [InverseProperty("TimeSheetDetail")]
    public virtual ICollection<TimeSheetDetailReimbursement> TimeSheetDetailReimbursements { get; set; } = new List<TimeSheetDetailReimbursement>();
}
