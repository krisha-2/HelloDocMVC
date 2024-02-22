using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HelloDocMVC.Entity.DataModels;

[Table("CaseTag")]
public partial class CaseTag
{
    [Key]
    public int CaseTagId { get; set; }

    [Column("Name ")]
    [StringLength(56)]
    public string Name { get; set; } = null!;
}
