using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HelloDocMVC.Entity.DataModels;

[Table("RequestClient")]
public partial class RequestClient
{
    [Key]
    public int RequestClientId { get; set; }

    public int RequestId { get; set; }

    [Column("RC_FirstName")]
    [StringLength(100)]
    public string RcFirstName { get; set; } = null!;

    [Column("RC_LastName")]
    [StringLength(100)]
    public string? RcLastName { get; set; }

    [StringLength(23)]
    public string? PhoneNumber { get; set; }

    [Column("Location ")]
    [StringLength(100)]
    public string? Location { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [Column("RegionId ")]
    public int? RegionId { get; set; }

    [StringLength(20)]
    public string? NotiMobile { get; set; }

    [StringLength(50)]
    public string? NotiEmail { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    [StringLength(50)]
    public string? Email { get; set; }

    [Column("strMonth")]
    [StringLength(20)]
    public string? StrMonth { get; set; }

    [Column("intYear")]
    public int? IntYear { get; set; }

    [Column("intDate")]
    public int? IntDate { get; set; }

    [Column(TypeName = "bit(100)")]
    public BitArray? IsMobile { get; set; }

    [StringLength(100)]
    public string? Street { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? State { get; set; }

    [Column("ZipCode ")]
    [StringLength(10)]
    public string? ZipCode { get; set; }

    public short? CommunicationType { get; set; }

    public short? RemindReservationCount { get; set; }

    [Column("RemindHouseCallCount ")]
    public short? RemindHouseCallCount { get; set; }

    public short? IsSetFollowupSent { get; set; }

    [Column("IP ")]
    [StringLength(20)]
    public string? Ip { get; set; }

    public short? IsReservationReminderSent { get; set; }

    [Precision(9, 0)]
    public decimal? Latitude { get; set; }

    [Precision(9, 0)]
    public decimal? Longitude { get; set; }

    [ForeignKey("RegionId")]
    [InverseProperty("RequestClients")]
    public virtual Region? Region { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("RequestClients")]
    public virtual Request Request { get; set; } = null!;
}
