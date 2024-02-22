using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HelloDocMVC.Entity.Models;

namespace HelloDocMVC.Repository.Repository.Interface
{
    public interface IAdminDashboard
    {
        public List<AdminDashboardList> NewRequestData();
    }
}
