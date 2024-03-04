using HelloDocMVC.Entity.Models;
using HelloDocMVC.Entity.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public List<ViewDocument> Document(int? requestid);
        public void UploadDoc(int RequestId, IFormFile? UploadFile);
    }
}
