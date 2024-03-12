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
        public enum Status
        {
            New=1,
                Pending=2,
                Active=3,
                Conclude=4,
                ToClose=5,
                UnPaid=6
        }
        public enum Status1
        {
            Unassigne = 1,
            Accepted, Cancelled, MDEnRoute, MDONSite, Conclude, CancelledByPatients, Closed, Unpaid, Clear,
            Block

        }
    }
}
