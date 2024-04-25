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
            Unassigned = 1,
            Accepted, Cancelled, MDEnRoute, MDONSite, Conclude, CancelledByPatients, Closed, Unpaid, Clear,
            Block
        }
        public enum AdminStatus
        {
            Active = 1,
            Pending,
            NotActive
        }
        public enum OnCallStatus
        {
            Unavailable = 0,
            Available
        }
        public enum AccountType
        {
            All = 0,
            Admin,
            Physician,
            Patient
        }
        public enum EmailAction
        {
            Sendorder = 1,
            Request,
            SendLink,
            SendAgreement,
            Forgot,
            NewRegistration,
            contact

        }
    }
}
