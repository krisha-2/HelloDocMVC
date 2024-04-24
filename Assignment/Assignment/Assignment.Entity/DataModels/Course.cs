using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Entity.DataModels;

[Table("Course")]
public partial class Course
{
    [Key]
    public int Id { get; set; }

    [StringLength(64)]
    public string? Name { get; set; }

    [InverseProperty("CourseNavigation")]
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
