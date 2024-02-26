using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Entity.Models
{
    public class Constant
    {
        public enum RequestType
        {
            Business=1,
            Patient,
            Family,
            Concierge
        }
    }
}
