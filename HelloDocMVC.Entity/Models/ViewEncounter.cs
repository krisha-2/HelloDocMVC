using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Entity.Models
{
    public class ViewEncounter
    {
        public int? RequestId { get; set; }
        public int? AdminId { get; set; }
        public int EncounterId { get; set; }
        public int? PhysicianId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string? LastName { get; set; }

        public string? Location { get; set; }

        public DateTime DOB { get; set; }

        public DateTime? DOS { get; set; }

        public string? Mobile { get; set; }

        public string Email { get; set; } = string.Empty;

        public string? Injury { get; set; }

        public string? History { get; set; }

        public string? Medications { get; set; }

        public string? Allergies { get; set; }

        public string? Temp { get; set; }

        public string? HR { get; set; }

        public string? RR { get; set; }

        public string? Bp { get; set; }
        public string? Bpd { get; set; }

        public string? O2 { get; set; }

        public string? Pain { get; set; }

        public string? Heent { get; set; }

        public string? CV { get; set; }

        public string? Chest { get; set; }

        public string? ABD { get; set; }

        public string? Extr { get; set; }

        public string? Skin { get; set; }

        public string? Neuro { get; set; }

        public string? Other { get; set; }

        public string? Diagnosis { get; set; }

        public string? Treatment { get; set; }

        public string? MDispensed { get; set; }

        public string? Procedures { get; set; }

        public string? Followup { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}

