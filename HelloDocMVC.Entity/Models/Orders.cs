using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Entity.Models
{
    public class Orders
    {
        public int? UserId { get; set; }
        public int RequestId { get; set; }
        public int VendorId { get; set; }
        public string? FaxNumber { get; set; }
        public string? Prescription { get; set; }
        public string? Business { get; set; }
        public string? Business_contact { get; set; }
        public int NoOfRefill { get; set; }
        public int? Fax_Number { get; set; }
        public string? FirstName { get; set; }
        public string Email { get; set; }
        public string select_profession { get; set; }
    }
}
