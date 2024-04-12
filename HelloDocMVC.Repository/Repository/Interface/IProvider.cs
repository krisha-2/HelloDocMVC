using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Repository.Repository.Interface
{
    public interface IProvider
    {
        public ViewProvider PhysicianAll(ViewProvider vp);
        public ViewProvider PhysicianByRegion(int? region, ViewProvider vp);
        public Task<bool> ChangeNotificationPhysician(Dictionary<int, bool> changedValuesDict);
        public Task<bool> PhysicianAddEdit(ViewProvider physiciandata, string AdminId);
        public Task<ViewProvider> GetPhysicianById(int id);
        public Task<bool> EditAccountInfo(ViewProvider vm);
        public Task<bool> ChangePasswordAsync(string password, int Physicianid);
        public Task<bool> EditPhysicianInfo(ViewProvider vm);
        public Task<bool> EditMailBillingInfo(ViewProvider vm, string AdminId);
        public Task<bool> EditProviderProfile(ViewProvider vm, string AdminId);
        public Task<bool> EditProviderOnbording(ViewProvider vm, string AdminId);
        public Task<bool> DeletePhysician(int PhysicianID, string AdminID);
        public List<PhysicianLocation> FindPhysicianLocation();
        public bool SendMessage(string? Message);
    }
}
