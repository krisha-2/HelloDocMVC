using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Entity.Models
{
    public class PartnersData
    {
        public string Profession { get; set; }
        public string Business { get; set; }
        public string Email { get; set; }
        public string FaxNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string BusinessNumber { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int ProfessionId { get; set; }
        public string BusinessContact { get; set; }
        public string BusinessName { get; set; }
    }
}
