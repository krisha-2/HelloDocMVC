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
        public PaginatedViewModel IndexData();
        public ViewCase ViewCaseData(int? id);
        public PaginatedViewModel GetRequests(string Status, PaginatedViewModel data);
        public bool CancelCase(int RequestID, string Note, string CaseTag);
        public bool BlockCase(int RequestID, string Note);
        public Task<bool> AssignProvider(int RequestId, int ProviderId, string notes);
        public Task<bool> TransferProvider(int RequestId, int ProviderId, string notes);
        //public ViewDocument ViewUpload(int requestid);
        //public void UploadDoc(ViewDocument v, IFormFile? UploadFile, int RequestId);
        //public Task<bool> DeleteDocumentByRequest(string ids);
        public Task<ViewDocuments> GetDocumentByRequest(int? id);
        public Boolean SaveDoc(int Requestid, IFormFile file);
        public Task<bool> DeleteDocumentByRequest(string ids);
        public bool ClearCase(int RequestID);
        //public bool Orders(Orders sm);
        public ViewNotesData getNotesByID(int id);
        public bool EditViewNotes(string? adminnotes, string? physiciannotes, int RequestID);
        public bool SendAgreement(int requestid);
        public bool SendAgreement_accept(int RequestID);
        public bool SendAgreement_Reject(int RequestID, string Notes);
        public ViewCloseCase CloseCaseData(int RequestID);
        public bool EditForCloseCase(ViewCloseCase model);
        public bool CloseCase(int RequestID);
        public ViewEncounter EncounterInfo(int id);
        public void EditEncounterinfo(ViewEncounter ve);
        List<AdminDashboardList> Export(string status);
        public void CreateRequest(Patient vdcp);
        public Boolean SendLink(string FirstName, string LastName, string Email);
    }
}
