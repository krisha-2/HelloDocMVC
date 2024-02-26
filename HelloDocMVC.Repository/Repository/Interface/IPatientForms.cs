using HelloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Repository.Repository.Interface
{
    public interface IPatientForms
    {
        public void PatientRequest(Patient viewdata);
        public void FamilyFriendRequest(FamilyFriend viewdata);
        public void ConciergeRequest(Concierge viewdata);
        public void BusinessRequest(Business viewdata);
    }
}
