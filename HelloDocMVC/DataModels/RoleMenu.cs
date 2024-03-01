using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HelloDocMVC.DataModels;

[Table("RoleMenu")]
public partial class RoleMenu
{
    [Key]
    public int RoleMenuId { get; set; }

    [Column("RoleId ")]
    public int RoleId { get; set; }

    [Column("MenuId ")]
    public int MenuId { get; set; }

    [ForeignKey("MenuId")]
    [InverseProperty("RoleMenus")]
    public virtual Menu Menu { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("RoleMenus")]
    public virtual Role Role { get; set; } = null!;
}
