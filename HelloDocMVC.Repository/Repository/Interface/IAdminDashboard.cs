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
        public CountStatusWiseRequestModel IndexData();
        public ViewCase ViewCaseData(int? id);
        public List<AdminDashboardList> GetRequests(string Status);
        public bool CancelCase(int RequestID, string Note, string CaseTag);
        public bool BlockCase(int RequestID, string Note);
        public Task<bool> AssignProvider(int RequestId, int ProviderId, string notes);
    }
}
