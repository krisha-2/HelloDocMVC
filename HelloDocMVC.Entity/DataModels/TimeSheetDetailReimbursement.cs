using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HelloDocMVC.Entity.DataModels;

[Table("TimeSheetDetailReimbursement")]
public partial class TimeSheetDetailReimbursement
{
    [Key]
    public int TimeSheetDetailReimbursementId { get; set; }

    public int TimeSheetDetailId { get; set; }

    [StringLength(500)]
    public string ItemName { get; set; } = null!;

    public int Amount { get; set; }

    [StringLength(500)]
    public string Bill { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    [StringLength(128)]
    public string CreatedBy { get; set; } = null!;

    [Column(TypeName = "timestamp without time zone")]
    public DateTime CreatedDate { get; set; }

    [StringLength(128)]
    public string? ModifiedBy { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("TimeSheetDetailReimbursementCreatedByNavigations")]
    public virtual AspNetUser CreatedByNavigation { get; set; } = null!;

    [ForeignKey("ModifiedBy")]
    [InverseProperty("TimeSheetDetailReimbursementModifiedByNavigations")]
    public virtual AspNetUser? ModifiedByNavigation { get; set; }

    [ForeignKey("TimeSheetDetailId")]
    [InverseProperty("TimeSheetDetailReimbursements")]
    public virtual TimeSheetDetail TimeSheetDetail { get; set; } = null!;
}
