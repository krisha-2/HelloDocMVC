using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Entity.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Repository.Repository.Interface
{
    public interface IRecords
    {
        public List<User> PatientHistory(string fname, string lname, string email, string phone);
        public List<PatientDashboardList> RecordsPatientExplore(int UserId);
        RecordsData GetFilteredSearchRecords(RecordsData rm);
        bool DeleteRequest(int? RequestId);
        RecordsData BlockHistory(RecordsData rm);
        bool Unblock(int RequestId, string id);
        public RecordsData GetFilteredSMSLogs(RecordsData rm);
        public RecordsData GetFilteredEmailLogs(RecordsData rm);
    }
}
