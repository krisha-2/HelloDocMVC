using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HelloDocMVC.Entity.Models.Constant;

namespace HelloDocMVC.Entity.Models.ViewModel
{
    public class PatientDashboardList
    {
        public DateTime createdDate { get; set; }
        public Status Status { get; set; }
        public int RequestId { get; set; }
        public int Fcount { get; set; }
    }
}