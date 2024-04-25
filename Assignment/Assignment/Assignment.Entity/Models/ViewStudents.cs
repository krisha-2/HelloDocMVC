using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Entity.Models
{
    public class ViewStudents
    {
        public int Id { get; set; }
        public int StudentId { get; set;}
        public string FirstName { get; set;}
        public string LastName { get; set;}
        public string Email { get; set;}
        public int Age { get; set;}
        public DateTime Dob { get; set;}
        public string Gender { get; set;}
        public string Course { get; set;}
        public string Grade { get; set;}
    }
    public class ViewPagination
    {
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchInput { get; set; }
        public bool? IsAscending { get; set; } = true;
        public string? SortedColumn { get; set; } = "Name";
        public List<ViewStudents> StudentList { get; set; }
    }
}
