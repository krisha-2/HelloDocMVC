using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Entity.Models
{
    public class Payrate
    {
        public int PhysicianId { get; set; }

        public int? NightShiftWeekend { get; set; }

        public int? Shift { get; set; }

        public int? HouseCallNightsWeekend { get; set; }

        public int? PhoneConsults { get; set; }

        public int? PhoneConsultsNightsWeekend { get; set; }

        public int? BatchTesting { get; set; }

        public int? HouseCalls { get; set; }
    }
}
