using HelloDocMVC.Entity.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Entity.Models
{
    public class SchedulingData
    {
        [Required(ErrorMessage = "Please select any region")]
        public int regionid { get; set; }
        [Required(ErrorMessage = "Please select any physician")]
        public int physicianid { get; set; }
        public string RegionName { get; set; }
        public DateTime shiftdate { get; set; }
        public TimeOnly starttime { get; set; }
        public TimeOnly endtime { get; set; }
        public int repeatcount { get; set; }
        public string physicianname { get; set; }
        public short status { get; set; }
        public string modaldate { get; set; }
        public int shiftdetailid { get; set; }
        public List<SchedulingData>? SD { get; set; }

        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public bool? IsAscending { get; set; } = true;
        public string? SortedColumn { get; set; } = "PhysicianName";
    }
    public class DayWiseScheduling
    {
        public DateTime date { get; set; }
        public List<Physician> physicians { get; set; }
        public List<ShiftDetail> shiftdetails { get; set; }
    }
    public class MonthWiseScheduling
    {
        public DateTime date { get; set; }
        public List<ShiftDetail> shiftdetails { get; set; }


    }
    public class WeekWiseScheduling
    {
        public DateTime date { get; set; }
        public List<Physician> physicians { get; set; }
        public List<ShiftDetail> shiftdetails { get; set; }
    }
}
