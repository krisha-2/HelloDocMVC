using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Repository.Repository.Interface
{
    public interface ILoginRepository
    {
        Task<UserInfo> CheckAccessLogin(AspNetUser aspNetUser);
        public bool SendResetLink(String Email);
        public bool isAccessGranted(int roleId, string menuName);
    }
}
