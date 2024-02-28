using HelloDocMVC.Entity.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Repository.Repository.Interface
{
    public interface IPatientDashboard
    {
        public List<PatientDashboardList> PatientDashboardList(int id);
    }
}
