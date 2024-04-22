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
        public PaginatedViewModel IndexData(int ProviderId);
        public ViewCase ViewCaseData(int? id);
        public PaginatedViewModel GetRequests(string Status, PaginatedViewModel data);
        public PaginatedViewModel GetRequests(string Status, PaginatedViewModel data, int ProviderId);
        public bool CancelCase(int RequestID, string Note, string CaseTag);
        public bool BlockCase(int RequestID, string Note);
        public Task<bool> AssignProvider(int RequestId, int ProviderId, string notes);
        public Task<bool> TransferProvider(int RequestId, int ProviderId, string notes);
        public Task<ViewDocuments> GetDocumentByRequest(ViewDocuments viewDocument, int? id);
        public Boolean SaveDoc(int Requestid, IFormFile file);
        public Task<bool> DeleteDocumentByRequest(string ids);
        public bool ClearCase(int RequestID);
        public bool SendOrder(Orders data);
        public ViewNotesData getNotesByID(int id);
        public bool EditViewNotes(string? adminnotes, string? physiciannotes, int RequestID);
        public bool SendAgreement(int requestid);
        public bool SendAgreement_accept(int RequestID);
        public bool SendAgreement_Reject(int RequestID, string Notes);
        public ViewCloseCase CloseCaseData(int RequestID);
        public bool EditForCloseCase(ViewCloseCase model);
        public bool CloseCase(int RequestID);
        //public ViewEncounter EncounterInfo(int id);
        //public void EditEncounterinfo(ViewEncounter ve);
        public ViewEncounter GetEncounterDetails(int RequestID);
        public bool EditEncounterDetails(ViewEncounter Data, string id);
        //public bool CaseFinalized(ViewEncounter Data);

        List<AdminDashboardList> Export(string status);
        public void CreateRequest(Patient vdcp);
        public Boolean SendLink(string FirstName, string LastName, string Email);
        public Task<bool> SendFileEmail(string ids, int Requestid, string email);
        public Task<bool> AcceptPhysician(int requestid, string note, int ProviderId);
        public Task<bool> TransfertoAdmin(int RequestID, string Note, int ProviderId);
        public bool CaseFinalized(ViewEncounter Data);
        public bool ContactAdmin(int ProviderId, string Notes);
        public bool Housecall(int RequestId);
        public bool Consult(int RequestId);
        public bool ConcludeCarePost(int RequestId, string Notes);
    }
}
