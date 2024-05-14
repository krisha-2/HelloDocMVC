using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HelloDocMVC.Entity.DataModels;

[Table("TimeSheet")]
public partial class TimeSheet
{
    [Key]
    public int TimeSheetId { get; set; }

    public int PhysicianId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool? IsFinalize { get; set; }

    public bool? IsApproved { get; set; }

    [StringLength(128)]
    public string? BonusAmount { get; set; }

    [StringLength(128)]
    public string CreatedBy { get; set; } = null!;

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [StringLength(128)]
    public string? ModifiedBy { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [StringLength(128)]
    public string? AdminNotes { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("TimeSheetCreatedByNavigations")]
    public virtual AspNetUser CreatedByNavigation { get; set; } = null!;

    [ForeignKey("ModifiedBy")]
    [InverseProperty("TimeSheetModifiedByNavigations")]
    public virtual AspNetUser? ModifiedByNavigation { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("TimeSheets")]
    public virtual Physician Physician { get; set; } = null!;

    [InverseProperty("TimeSheet")]
    public virtual ICollection<TimeSheetDetail> TimeSheetDetails { get; set; } = new List<TimeSheetDetail>();
}
