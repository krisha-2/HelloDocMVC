using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HelloDocMVC.DataModels;

[Table("RequestClosed")]
public partial class RequestClosed
{
    [Key]
    public int RequestClosedId { get; set; }

    public int RequestId { get; set; }

    public int RequestStatusLogId { get; set; }

    public string? PhyNotes { get; set; }

    public string? ClientNotes { get; set; }

    [Column("IP")]
    public string? Ip { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("RequestCloseds")]
    public virtual Request Request { get; set; } = null!;

    [ForeignKey("RequestStatusLogId")]
    [InverseProperty("RequestCloseds")]
    public virtual RequestStatusLog RequestStatusLog { get; set; } = null!;
}
