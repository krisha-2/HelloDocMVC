using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Entity.Models
{
    public class AdminDashboardList
    {
        public int RequestId { get; set; }
        public string PatientName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int RequestTypeId { get; set; }
        public string Requestor { get; set; }
        public DateTime RequestedDate { get; set; }
        public string PatientPhoneNumber { get; set; }
        public string RequestorPhoneNumber { get; set; }
        public string Address { get; set; }
        public string Notes { get; set; }
        public string Email { get; set; }
    }
}
