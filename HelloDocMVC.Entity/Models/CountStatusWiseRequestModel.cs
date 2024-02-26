using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Entity.Models
{
    public class CountStatusWiseRequestModel
    {
        public int NewRequest { get; set; }
        public int PendingRequest { get; set; }
        public int ActiveRequest { get; set; }
        public int ConcludeRequest { get; set; }
        public int ToCloseRequest { get; set; }
        public int UnpaidRequest { get; set; }
        public List<AdminDashboardList> adminDashboardList { get; set; }
    }
}
