using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HelloDocMVC.DataModels;

[Table("PhysicianRegion")]
public partial class PhysicianRegion
{
    [Key]
    [Column("PhysicianRegionId ")]
    public int PhysicianRegionId { get; set; }

    public int PhysicianId { get; set; }

    [Column("RegionId ")]
    public int RegionId { get; set; }

    [ForeignKey("PhysicianId")]
    [InverseProperty("PhysicianRegions")]
    public virtual Physician Physician { get; set; } = null!;

    [ForeignKey("RegionId")]
    [InverseProperty("PhysicianRegions")]
    public virtual Region Region { get; set; } = null!;
}
