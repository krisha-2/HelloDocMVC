using HelloDocMVC.Entity.DataModels;
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
    }
}
