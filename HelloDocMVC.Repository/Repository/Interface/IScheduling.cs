using HelloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Repository.Repository.Interface
{
    public interface IScheduling
    {
        public void AddShift(SchedulingData model, List<string?>? chk, string adminId);
        public void ViewShift(int shiftdetailid);
        public void ViewShiftreturn(SchedulingData modal);
        public bool ViewShiftSave(SchedulingData modal, string id);
        public bool ViewShiftDelete(SchedulingData modal, string id);
        public Task<List<ViewProvider>> PhysicianOnCall(int? region);
        public Task<List<SchedulingData>> GetAllNotApprovedShift(int? regionId);
        public Task<bool> DeleteShift(string s, string AdminID);
        public Task<bool> UpdateStatusShift(string s, string AdminID);
    }
}
