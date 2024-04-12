using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Entity.Models
{
    public class SearchRecords
    {
        public int RequestId { get; set; }
        public int RequestTypeId { get; set; }
        public string PatientName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public short Status { get; set; }
        public string PhysicianName { get; set; }
        public string PhysicianNote { get; set; }
        public string? CancelByProviderNote { get; set; }
        public string AdminNote { get; set; }
        public string PatientNote { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime DateOfService { get; set; }
        public DateTime? CloseCaseDate { get; set; }

    }
}
