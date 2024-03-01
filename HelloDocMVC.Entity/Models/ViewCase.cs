using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Entity.Models
{
    public class ViewCase
    {
        public int? UserId { get; set; }

        public string? AspNetUserId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }
        public int? RequestId { get; set; }

        public string Email { get; set; }

        public string? Mobile { get; set; }

        public BitArray? Ismobile { get; set; }

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public int? RegionId { get; set; }

        public string? ZipCode { get; set; }

        public string? StrMonth { get; set; }
        public DateTime DOB { get; set; }

        public int? IntYear { get; set; }

        public int? IntDate { get; set; }

        public string CreatedBy { get; set; } = null!;

        public DateTime CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public short? Status { get; set; }

        public string? Ip { get; set; }
        public int RequestTypeId { get; set; }
        public string ConfNo { get; set; }
        public string Address { get; set;}
        public string Symptoms { get; set; }

    }
}