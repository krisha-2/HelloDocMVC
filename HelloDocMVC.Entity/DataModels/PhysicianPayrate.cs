using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HelloDocMVC.Entity.DataModels;

[Table("PhysicianPayrate")]
public partial class PhysicianPayrate
{
    [Key]
    public int PhysicianId { get; set; }

    public int? NightShiftWeekend { get; set; }

    public int? Shift { get; set; }

    public int? HouseCallNightsWeekend { get; set; }

    public int? PhoneConsults { get; set; }

    public int? PhoneConsultsNightsWeekend { get; set; }

    public int? BatchTesting { get; set; }

    public int? HouseCalls { get; set; }

    [Column(TypeName = "character varying")]
    public string CreatedBy { get; set; } = null!;

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    [Column(TypeName = "character varying")]
    public string? ModifiedBy { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("PhysicianPayrateCreatedByNavigations")]
    public virtual AspNetUser CreatedByNavigation { get; set; } = null!;

    [ForeignKey("ModifiedBy")]
    [InverseProperty("PhysicianPayrateModifiedByNavigations")]
    public virtual AspNetUser? ModifiedByNavigation { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("PhysicianPayrate")]
    public virtual Physician Physician { get; set; } = null!;
}
