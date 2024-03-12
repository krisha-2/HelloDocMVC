using HelloDocMVC.Repository.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelloDocMVC.Entity.Models.ViewModel;
using HelloDocMVC.Entity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public Task<bool> TransferProvider(int RequestId, int ProviderId, string notes);
        public ViewDocument ViewUpload(int requestid);
        public void UploadDoc(ViewDocument v, IFormFile? UploadFile);
        public bool ClearCase(int RequestID);
        //public bool Orders(Orders sm);
        public ViewNotesData getNotesByID(int id);
        public bool EditViewNotes(string? adminnotes, string? physiciannotes, int RequestID);
        public Boolean SendAgreement(int requestid);
        public Boolean SendAgreement_accept(int RequestID);
        public Boolean SendAgreement_Reject(int RequestID, string Notes);
    }
}
