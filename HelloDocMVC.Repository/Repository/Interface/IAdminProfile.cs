using HelloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Repository.Repository.Interface
{
    public interface IAdminProfile
    {
        public Task<ViewAdminProfileData> GetProfileDetails(int UserId);
        public Task<bool> EditPassword(string password, int adminId);
        public Task<bool> EditAdministratorInfo(ViewAdminProfileData _viewAdminProfile);
        public Task<bool> BillingInfoEdit(ViewAdminProfileData _viewAdminProfile);
    }
}
