using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Entity.DataModels;

[Table("Student")]
public partial class Student
{
    [Key]
    public int Id { get; set; }

    [StringLength(32)]
    public string? FirstName { get; set; }

    [StringLength(32)]
    public string? LastName { get; set; }

    public int? CourseId { get; set; }

    public int? Age { get; set; }

    [StringLength(64)]
    public string? Email { get; set; }

    [StringLength(10)]
    public string? Gender { get; set; }

    [StringLength(64)]
    public string? Course { get; set; }

    [StringLength(32)]
    public string? Grade { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? IsDeleted { get; set; }

    [ForeignKey("CourseId")]
    [InverseProperty("Students")]
    public virtual Course? CourseNavigation { get; set; }
}
