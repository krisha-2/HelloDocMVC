using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Net;
using static HelloDocMVC.Entity.Models.Constant;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using System.Collections;
using Microsoft.AspNetCore.Components.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HelloDocMVC.Repository.Repository
{
    public class AdminDashboard : IAdminDashboard
    {
        private readonly HelloDocDbContext _context;
        private readonly EmailConfiguration _emailConfig;
        public AdminDashboard(HelloDocDbContext context, EmailConfiguration emailConfig)
        {
            _context = context;
            _emailConfig = emailConfig;
        }
        public List<AdminDashboardList> NewRequestData()
        {
            var list = _context.Requests.Join
                       (_context.RequestClients,
                       requestclients => requestclients.RequestId, requests => requests.RequestId,
                       (requests, requestclients) => new { Request = requests, Requestclient = requestclients }
                       )
                       .Where(req => req.Request.Status == 1)
                       .Select(req => new AdminDashboardList()
                       {
                           RequestId = req.Request.RequestId,
                           PatientName = req.Requestclient.RcFirstName + " " + req.Requestclient.RcLastName,
                           Email = req.Requestclient.Email,
                           DateOfBirth = new DateTime((int)req.Requestclient.IntYear, Convert.ToInt32(req.Requestclient.StrMonth.Trim()), (int)req.Requestclient.IntDate),
                           RequestTypeId = req.Request.RequestTypeId,
                           Requestor = req.Request.RFirstName + " " + req.Request.RLastName,
                           RequestedDate = req.Request.CreatedDate,
                           PatientPhoneNumber = req.Requestclient.PhoneNumber,
                           RequestorPhoneNumber = req.Request.PhoneNumber,
                           Notes = req.Requestclient.Notes,
                           Address = req.Requestclient.Address + " " + req.Requestclient.Street + " " + req.Requestclient.City + " " + req.Requestclient.State + " " + req.Requestclient.ZipCode
                       })
                       .OrderBy(req => req.RequestedDate)
                        .ToList();
            return list;
        }
        public PaginatedViewModel IndexData()
        {
            return new PaginatedViewModel
            {
                NewRequest = _context.Requests.Where(r => r.Status == 1).Count(),
                PendingRequest = _context.Requests.Where(r => r.Status == 2).Count(),
                ActiveRequest = _context.Requests.Where(r => (r.Status == 4 || r.Status == 5)).Count(),
                ConcludeRequest = _context.Requests.Where(r => r.Status == 6).Count(),
                ToCloseRequest = _context.Requests.Where(r => (r.Status == 3 || r.Status == 7 || r.Status == 8)).Count(),
                UnpaidRequest = _context.Requests.Where(r => r.Status == 9).Count(),
                adl = NewRequestData()
            };
        }
        public ViewCase ViewCaseData(int? id)
        {
            ViewCase? viewCase = _context.Requests.Join
                       (_context.RequestClients,
                       requestclients => requestclients.RequestId, requests => requests.RequestId,
                       (requests, requestclients) => new { Request = requests, Requestclient = requestclients }
                       )
                       .Where(R => R.Request.RequestId == id)
                       .Select(req => new ViewCase()
                       {
                           RequestId = id,
                           RequestTypeId = req.Request.RequestTypeId,
                           ConfNo = req.Requestclient.City.Substring(0, 2) + req.Requestclient.IntDate.ToString() + req.Requestclient.StrMonth + req.Requestclient.IntYear.ToString() + req.Requestclient.RcLastName.Substring(0, 2) + req.Requestclient.RcFirstName.Substring(0, 2) + "002",
                           Symptoms = req.Requestclient.Notes,
                           FirstName = req.Requestclient.RcFirstName,
                           LastName = req.Requestclient.RcLastName,
                           DOB = new DateTime((int)req.Requestclient.IntYear, Convert.ToInt32(req.Requestclient.StrMonth.Trim()), (int)req.Requestclient.IntDate),
                           Mobile = req.Requestclient.PhoneNumber,
                           Email = req.Requestclient.Email,
                           Address = req.Requestclient.Address

                       }).FirstOrDefault();
            return viewCase;
        }
        public PaginatedViewModel GetRequests(string Status, PaginatedViewModel data)
        {
            if (data.SearchInput != null)
            {
                data.SearchInput = data.SearchInput.Trim();
            }
            List<int> statusdata = Status.Split(',').Select(int.Parse).ToList();
            List<AdminDashboardList> allData = (from req in _context.Requests
                                                join reqClient in _context.RequestClients
                                                on req.RequestId equals reqClient.RequestId into reqClientGroup
                                                from rc in reqClientGroup.DefaultIfEmpty()
                                                join phys in _context.Physicians
                                                on req.PhysicianId equals phys.PhysicianId into physGroup
                                                from p in physGroup.DefaultIfEmpty()
                                                join reg in _context.Regions
                                                on rc.RegionId equals reg.RegionId into RegGroup
                                                from rg in RegGroup.DefaultIfEmpty()
                                                where statusdata.Contains(req.Status) && (data.SearchInput == null ||
                                                         rc.RcFirstName.Contains(data.SearchInput) || rc.RcLastName.Contains(data.SearchInput) ||
                                                         req.RFirstName.Contains(data.SearchInput) || req.RLastName.Contains(data.SearchInput) ||
                                                         rc.Email.Contains(data.SearchInput) || rc.PhoneNumber.Contains(data.SearchInput) ||
                                                         rc.Address.Contains(data.SearchInput) || rc.Notes.Contains(data.SearchInput) ||
                                                         p.FirstName.Contains(data.SearchInput) || p.LastName.Contains(data.SearchInput) ||
                                                         rg.Name.Contains(data.SearchInput)) && (data.RegionId == null || rc.RegionId == data.RegionId)
                                                         && (data.RequestType == null || req.RequestTypeId == data.RequestType)
                                                orderby req.CreatedDate descending
                                                select new AdminDashboardList
                                                {

                                                    RequestId = req.RequestId,
                                                    RequestTypeId = req.RequestTypeId,
                                                    Requestor = req.RFirstName + " " + req.RLastName,
                                                    PatientName = rc.RcFirstName + " " + rc.RcLastName,
                                                    //DateOfBirth = new DateOnly((int)rc.IntYear, DateTime.ParseExact(rc.StrMonth, "MMMM", new CultureInfo("en-US")).Month, (int)rc.IntDate),
                                                    //DateOfBirth = new DateTime((int)rc.IntYear, int.Parse(rc.StrMonth), (int)rc.IntDate),
                                                    //DateOfBirth = new DateTime((int)rc.IntYear, Convert.ToInt32(rc.StrMonth.Trim()), (int)rc.IntDate),
                                                    IntYear = rc.IntYear,
                                                    StrMonth = rc.StrMonth,
                                                    IntDate = rc.IntDate,
                                                    RequestedDate = req.CreatedDate,
                                                    Email = rc.Email,
                                                    RegionId = rg.Name,
                                                    ProviderName = p.FirstName + " " + p.LastName,
                                                    PhoneNumber = rc.PhoneNumber,
                                                    Address = rc.Address + "," + rc.Street + "," + rc.City + "," + rc.State + "," + rc.ZipCode,
                                                    Notes = rc.Notes,
                                                    //ProviderId = req.PhysicianId,
                                                    RequestorPhoneNumber = req.PhoneNumber
                                                }).ToList();
            int totalItemCount = allData.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)data.PageSize);
            List<AdminDashboardList> list1 = allData.Skip((data.CurrentPage - 1) * data.PageSize).Take(data.PageSize).ToList();
            
            PaginatedViewModel paginatedViewModel = new PaginatedViewModel
            {
                adl = list1,
                CurrentPage = data.CurrentPage,
                TotalPages = totalPages,
                PageSize = data.PageSize,
                SearchInput = data.SearchInput
            };
            return paginatedViewModel;
           
        }
        public bool CancelCase(int RequestID, string Note, string CaseTag)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.RequestId == RequestID);
                if (requestData != null)
                {
                    requestData.CaseTag = CaseTag;
                    requestData.Status = 3;
                    _context.Requests.Update(requestData);
                    _context.SaveChanges();
                    RequestStatusLog rsl = new RequestStatusLog
                    {
                        RequestId = RequestID,
                        Notes = Note,
                        Status = 8,
                        CreatedDate = DateTime.Now
                    };
                    _context.RequestStatusLogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool BlockCase(int RequestID, string Note)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.RequestId == RequestID);
                if (requestData != null)
                {
                    requestData.Status = 11;
                    _context.Requests.Update(requestData);
                    _context.SaveChanges();
                    BlockRequest blc = new BlockRequest
                    {
                        RequestId = requestData.RequestId,
                        PhoneNumber = requestData.PhoneNumber,
                        Email = requestData.Email,
                        Reason = Note,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };
                    _context.BlockRequests.Add(blc);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> AssignProvider(int RequestId, int ProviderId, string notes)
        {

            var request = await _context.Requests.FirstOrDefaultAsync(req => req.RequestId == RequestId);
            request.PhysicianId = ProviderId;
            request.Status = 2;
            _context.Requests.Update(request);
            _context.SaveChanges();

            RequestStatusLog rsl = new RequestStatusLog();
            rsl.RequestId = RequestId;
            rsl.PhysicianId = ProviderId;
            rsl.Notes = notes;

            rsl.CreatedDate = DateTime.Now;
            rsl.Status = 2;
            _context.RequestStatusLogs.Update(rsl);
            _context.SaveChanges();

            return true;
        }
        public async Task<bool> TransferProvider(int RequestId, int ProviderId, string notes)
        {
            var request = await _context.Requests.FirstOrDefaultAsync(req => req.RequestId == RequestId);

            RequestStatusLog rsl = new RequestStatusLog();
            rsl.RequestId = RequestId;
            rsl.PhysicianId = request.PhysicianId;
            rsl.Notes = notes;
            rsl.CreatedDate = DateTime.Now;
            rsl.TransToPhysicianId = ProviderId;
            rsl.Status = 2;
            _context.RequestStatusLogs.Update(rsl);
            _context.SaveChanges();

            request.PhysicianId = ProviderId;
            request.Status = 2;
            _context.Requests.Update(request);
            _context.SaveChanges();
            return true;
        }
        public bool ClearCase(int RequestID)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.RequestId == RequestID);
                if (requestData != null)
                {
                    requestData.Status = 10;
                    _context.Requests.Update(requestData);
                    _context.SaveChanges();
                    RequestStatusLog rsl = new RequestStatusLog
                    {
                        RequestId = RequestID,
                        Status = 10,
                        CreatedDate = DateTime.Now
                    };
                    _context.RequestStatusLogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public ViewDocument ViewUpload(int requestid)
        {
            ViewDocument? items = _context.RequestClients.Include(rc => rc.Request)
                                  .Where(rc => rc.RequestId == requestid)
                                    .Select(rc => new ViewDocument()
                                    {
                                        FirstName = rc.RcFirstName,
                                        LastName = rc.RcLastName,
                                        ConfirmationNumber = rc.Request.ConfirmationNumber

                                    }).FirstOrDefault();
            List<RequestWiseFile> list = _context.RequestWiseFiles
                      .Where(r => r.RequestId == requestid && r.IsDeleted == new BitArray(1))
                      .OrderByDescending(x => x.CreatedDate)
                      .Select(r => new RequestWiseFile
                      {
                          CreatedDate = r.CreatedDate,
                          FileName = r.FileName,
                          RequestWiseFileId = r.RequestWiseFileId,
                          RequestId = r.RequestId,
                          IsDeleted = new BitArray(1)

                      }).ToList();
            items.Files = list;
            return items;
        }
        public void UploadDoc(ViewDocument vp, IFormFile? UploadFile)
        {
            string UploadImage;
            if (UploadFile != null)
            {
                string FilePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                string fileNameWithPath = Path.Combine(path, UploadFile.FileName);
                UploadImage = "~" + FilePath.Replace("wwwroot\\", "/") + "/" + UploadFile.FileName;
                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    UploadFile.CopyTo(stream);
                }
                var requestwisefile = new RequestWiseFile
                {
                    RequestId = vp.RequestId,
                    FileName = UploadFile.FileName,
                    CreatedDate = DateTime.Now,
                    IsDeleted = new BitArray(1)
                };
                _context.RequestWiseFiles.Add(requestwisefile);
                _context.SaveChanges();
            }
        }
        public async Task<bool> DeleteDocumentByRequest(string ids)
        {
            List<int> deletelist = ids.Split(',').Select(int.Parse).ToList();
            foreach (int item in deletelist)
            {
                if (item > 0)
                {
                    var data = await _context.RequestWiseFiles.Where(e => e.RequestWiseFileId == item).FirstOrDefaultAsync();
                    if (data != null)
                    {
                        data.IsDeleted[0] = true;
                        _context.RequestWiseFiles.Update(data);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public async Task<bool> Business(int RequestId, int ProviderId, string notes)
        {

            var request = await _context.Requests.FirstOrDefaultAsync(req => req.RequestId == RequestId);
            request.PhysicianId = ProviderId;
            request.Status = 2;
            _context.Requests.Update(request);
            _context.SaveChanges();

            RequestStatusLog rsl = new RequestStatusLog();
            rsl.RequestId = RequestId;
            rsl.PhysicianId = ProviderId;
            rsl.Notes = notes;

            rsl.CreatedDate = DateTime.Now;
            rsl.Status = 2;
            _context.RequestStatusLogs.Update(rsl);
            _context.SaveChanges();

            return true;
        }
        public ViewNotesData getNotesByID(int id)
        {
            var req = _context.Requests.FirstOrDefault(W => W.RequestId == id);
            var symptoms = _context.RequestClients.FirstOrDefault(W => W.RequestId == id);
            var transferlog = (from rs in _context.RequestStatusLogs
                               join py in _context.Physicians on rs.PhysicianId equals py.PhysicianId into pyGroup
                               from py in pyGroup.DefaultIfEmpty()
                               join p in _context.Physicians on rs.TransToPhysicianId equals p.PhysicianId into pGroup
                               from p in pGroup.DefaultIfEmpty()
                               join a in _context.Admins on rs.AdminId equals a.AdminId into aGroup
                               from a in aGroup.DefaultIfEmpty()
                               where rs.RequestId == id && rs.Status == 2
                               select new TransferNotesData
                               {
                                   
                                   TransPhysician = p.FirstName,
                                   Admin = a.FirstName,
                                   Physician = py.FirstName,
                                   Requestid = rs.RequestId,
                                   Notes = rs.Notes,
                                   Status = rs.Status,
                                   Physicianid = rs.PhysicianId,
                                   Createddate = rs.CreatedDate,
                                   Requeststatuslogid = rs.RequestStatusLogId,
                                   Transtoadmin = rs.TransToAdmin,
                                   Transtophysicianid = rs.TransToPhysicianId
                               }).ToList();
            var cancelbyprovider = _context.RequestStatusLogs.Where(E => E.RequestId == id && (E.TransToAdmin != null));
            var cancel = _context.RequestStatusLogs.Where(E => E.RequestId == id && (E.Status == 7 || E.Status == 3));
            var model = _context.RequestNotes.FirstOrDefault(E => E.RequestId == id);
            ViewNotesData allData = new ViewNotesData();
            allData.Requestid = id;
            //allData.Patientnotes = symptoms.Notes;
            if (model == null)
            {
                allData.Physiciannotes = "-";
                allData.Adminnotes = "-";
            }
            else
            {
                allData.Status = req.Status;
                allData.Requestnotesid = model.RequestNotesId;
                allData.Physiciannotes = model.PhysicianNotes ?? "-";
                allData.Adminnotes = model.AdminNotes ?? "-";
            }

            List<TransferNotesData> transfer = new List<TransferNotesData>();
            foreach (var item in transferlog)
            {
                transfer.Add(new TransferNotesData
                {
                    TransPhysician = item.TransPhysician,
                    Admin = item.Admin,
                    Physician = item.Physician,
                    Requestid = item.Requestid,
                    Notes = item.Notes ?? "-",
                    Status = item.Status,
                    Physicianid = item.Physicianid,
                    Createddate = item.Createddate,
                    Requeststatuslogid = item.Requeststatuslogid,
                    Transtoadmin = item.Transtoadmin,
                    Transtophysicianid = item.Transtophysicianid
                });
            }
            allData.transfernotes = transfer;
            List<TransferNotesData> cancelbyphysician = new List<TransferNotesData>();
            foreach (var item in cancelbyprovider)
            {
                cancelbyphysician.Add(new TransferNotesData
                {
                    Requestid = item.RequestId,
                    Notes = item.Notes ?? "-",
                    Status = item.Status,
                    Physicianid = item.PhysicianId,
                    Createddate = item.CreatedDate,
                    Requeststatuslogid = item.RequestStatusLogId,
                    Transtoadmin = item.TransToAdmin,
                    Transtophysicianid = item.TransToPhysicianId
                });
            }
            allData.cancelbyphysician = cancelbyphysician;

            List<TransferNotesData> cancelrq = new List<TransferNotesData>();
            foreach (var item in cancel)
            {
                cancelrq.Add(new TransferNotesData
                {
                    Requestid = item.RequestId,
                    Notes = item.Notes ?? "-",
                    Status = item.Status,
                    Physicianid = item.PhysicianId,
                    Createddate = item.CreatedDate,
                    Requeststatuslogid = item.RequestStatusLogId,
                    Transtoadmin = item.TransToAdmin,
                    Transtophysicianid = item.TransToPhysicianId
                });
            }
            allData.cancel = cancelrq;

            return allData;
        }
        public bool EditViewNotes(string? adminnotes, string? physiciannotes, int RequestID)
        {
            try
            {
                RequestNote notes = _context.RequestNotes.FirstOrDefault(E => E.RequestId == RequestID);
                if (notes != null)
                {
                    if (physiciannotes != null)
                    {
                        if (notes != null)
                        {
                            notes.PhysicianNotes = physiciannotes;
                            notes.ModifiedDate = DateTime.Now;
                            _context.RequestNotes.Update(notes);
                            _context.SaveChangesAsync();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (adminnotes != null)
                    {
                        if (notes != null)
                        {
                            notes.AdminNotes = adminnotes;
                            notes.ModifiedDate = DateTime.Now;
                            _context.RequestNotes.Update(notes);
                            _context.SaveChangesAsync();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    RequestNote rn = new RequestNote
                    {
                        RequestId = RequestID,
                        AdminNotes = adminnotes,
                        PhysicianNotes = physiciannotes,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "001e35a5 - cd12 - 4ec8 - a077 - 95db9d54da0f"
                    };
                    _context.RequestNotes.Add(rn);
                    _context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //public void SendOrder()
        //{

        //    OrderDetails od = new OrderDetails();

        //    //var isexist = _context.Users.FirstOrDefault(x => x.Email == viewdata.Email);
        //    //AspNetUser
        //        od.
        //        //A.UserName = viewdata.FirstName + " " + viewdata.LastName;
        //        //A.Email = viewdata.Email;
        //        //A.PhoneNumber = viewdata.PhoneNumber;
        //        //A.CreatedDate = DateTime.Now;
        //        _context.Add(od);
        //        _context.SaveChanges();
        //}
        #region SendAgreement
        public bool SendAgreement(int requestid)
        {
            var res = _context.RequestClients.FirstOrDefault(e => e.RequestId == requestid);
            var agreementUrl = "https://localhost:44338/AgreementPage?RequestID=" + requestid;
            _emailConfig.SendMail(res.Email, "Agreement for your request", $"<a href='{agreementUrl}'>Agree/Disagree</a>");
            return true;
        }
        #endregion

        #region SendAgreement_accept
        public bool SendAgreement_accept(int RequestID)
        {
            var request = _context.Requests.Find(RequestID);
            if (request != null)
            {
                request.Status = 4;
                _context.Requests.Update(request);
                _context.SaveChanges();

                RequestStatusLog rsl = new RequestStatusLog();
                rsl.RequestId = RequestID;

                rsl.Status = 4;

                rsl.CreatedDate = DateTime.Now;

                _context.RequestStatusLogs.Add(rsl);
                _context.SaveChanges();

            }
            return true;
        }
        #endregion

        #region SendAgreement_Reject
        public bool SendAgreement_Reject(int RequestID, string Notes)
        {
            var request = _context.Requests.Find(RequestID);
            if (request != null)
            {
                request.Status = 7;
                _context.Requests.Update(request);
                _context.SaveChanges();

                RequestStatusLog rsl = new RequestStatusLog();
                rsl.RequestId = RequestID;

                rsl.Status = 7;
                rsl.Notes = Notes;

                rsl.CreatedDate = DateTime.Now;

                _context.RequestStatusLogs.Add(rsl);
                _context.SaveChanges();

            }
            return true;
        }
        #endregion

        public ViewCloseCase CloseCaseData(int RequestID)
        {
            ViewCloseCase alldata = new ViewCloseCase();

            var result = from requestWiseFile in _context.RequestWiseFiles
                         join request in _context.Requests on requestWiseFile.RequestId equals request.RequestId
                         join physician in _context.Physicians on request.PhysicianId equals physician.PhysicianId into physicianGroup
                         from phys in physicianGroup.DefaultIfEmpty()
                         join admin in _context.Admins on requestWiseFile.AdminId equals admin.AdminId into adminGroup
                         from adm in adminGroup.DefaultIfEmpty()
                         where request.RequestId == RequestID
                         select new
                         {

                             Uploader = requestWiseFile.PhysicianId != null ? phys.FirstName :
                             (requestWiseFile.AdminId != null ? adm.FirstName : request.RFirstName),
                             requestWiseFile.FileName,
                             requestWiseFile.CreatedDate,
                             requestWiseFile.RequestWiseFileId

                         };
            List<ViewDocument> doc = new List<ViewDocument>();
            foreach (var item in result)
            {
                doc.Add(new ViewDocument
                {
                    CreatedDate = item.CreatedDate,
                    FileName = item.FileName,
                    FirstName = item.Uploader,
                    RequestWiseFileId = item.RequestWiseFileId

                });

            }
            alldata.documentslist = doc;
            Request req = _context.Requests.FirstOrDefault(r => r.RequestId == RequestID);

            alldata.Firstname = req.RFirstName;
            alldata.RequestID = req.RequestId;
            alldata.ConfirmationNumber = req.ConfirmationNumber;
            alldata.Lastname = req.RLastName;

            var reqcl = _context.RequestClients.FirstOrDefault(e => e.RequestId == RequestID);

            alldata.RC_Email = reqcl.Email;
            //alldata.RC_Dob = new DateTime((int)reqcl.IntYear, DateTime.ParseExact(reqcl.StrMonth, "MMMM", new CultureInfo("en-US")).Month, (int)reqcl.IntDate);
            alldata.RC_FirstName = reqcl.RcFirstName;
            //DOB = new DateTime((int)req.Requestclient.IntYear, Convert.ToInt32(req.Requestclient.StrMonth.Trim()), (int)req.Requestclient.IntDate),
            //alldata.IntYear = rc.IntYear,
            //StrMonth = rc.StrMonth,
            //IntDate = rc.IntDate,
            alldata.RC_LastName = reqcl.RcLastName;
            alldata.RC_PhoneNumber = reqcl.PhoneNumber;
            return alldata;
        }
        public bool EditForCloseCase(ViewCloseCase model)
        {
            try
            {
                RequestClient client = _context.RequestClients.FirstOrDefault(E => E.RequestId == model.RequestID);
                if (client != null)
                {
                    client.PhoneNumber = model.RC_PhoneNumber;
                    client.RcFirstName = model.RC_FirstName;
                    client.RcLastName = model.RC_LastName;
                    //client.Email = model.RC_Email;
                    _context.RequestClients.Update(client);
                    _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool CloseCase(int RequestID)
        {
            try
            {
                var requestData = _context.Requests.FirstOrDefault(e => e.RequestId == RequestID);
                if (requestData != null)
                {

                    requestData.Status = 9;
                    requestData.ModifiedDate = DateTime.Now;

                    _context.Requests.Update(requestData);
                    _context.SaveChanges();

                    RequestStatusLog rsl = new RequestStatusLog
                    {
                        RequestId = RequestID,


                        Status = 9,
                        CreatedDate = DateTime.Now

                    };
                    _context.RequestStatusLogs.Add(rsl);
                    _context.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        //public ViewEncounter ViewEncounter(int? id, int adminid)
        //{

        //    EncounterForm ef = new EncounterForm();
        //    ef.AdminId = adminid;
        //    ef.RequestId = id;
        //    _context.EncounterForms.Add(ef);
        //    _context.SaveChanges();
        //    var n = _context.Requests.FirstOrDefault(E => E.RequestId == id);
        //    var e = _context.EncounterForms.FirstOrDefault(E => E.RequestId == id);
        //    var rc = _context.RequestClients.FirstOrDefault(R => R.RequestId == id);
        //    ViewEncounter? requestforviewcase = new ViewEncounter
        //    {
        //        AdminId= adminid,
        //        RequestId = id,
        //        FirstName = n.RFirstName,
        //        LastName = n.RLastName,
        //        Mobile = n.PhoneNumber,
        //        Email = n.Email,
        //        Location = rc.Street + "," + rc.City + "," + rc.State,
        //        //Bdate = new DateTime((int)rc.IntYear, DateTime.ParseExact(rc.StrMonth, "MMMM", new CultureInfo("en-US")).Month, (int)rc.IntDate),
        //        DOS = n.CreatedDate,
        //        Injury = e.HistoryOfPresentIllnessOrInjury,
        //        History = e.MedicalHistory,
        //        Medications = e.Medications,
        //        Allergies = e.Allergies,
        //        Temp = e.Temp,
        //        HR = e.Hr,
        //        RR = e.Rr,
        //        Bp = e.BloodPressureSystolic,
        //        Bpd = e.BloodPressureDiastolic,
        //        O2 = e.O2,
        //        Chest = e.Chest,
        //        ABD = e.Abd,
        //        Extr = e.Extremeties,
        //        Skin = e.Skin,
        //        Neuro = e.Neuro,
        //        Other = e.Other,
        //        Diagnosis = e.Diagnosis,
        //        Treatment = e.TreatmentPlan,
        //        MDispensed = e.MedicationsDispensed,
        //        Procedures = e.Procedures,
        //        Followup = e.FollowUp
        //    };


        //    return requestforviewcase;
        //}
        //public void EncounterinfoPost(ViewEncounter _viewencounterinfo, int adminid)
        //{
        //    var encform = new EncounterForm();
        //    var isexist = _context.EncounterForms.FirstOrDefault(x => x.AdminId == _viewencounterinfo.AdminId);
        //    if (isexist == null)
        //    {
        //        encform.HistoryOfPresentIllnessOrInjury = _viewencounterinfo.Injury;
        //        encform.Rr = _viewencounterinfo.RR;
        //        encform.Hr = _viewencounterinfo.HR;
        //        encform.Skin = _viewencounterinfo.Skin;
        //        encform.Other = _viewencounterinfo.Other;
        //        encform.Procedures = _viewencounterinfo.Procedures;
        //        encform.Neuro = _viewencounterinfo.Neuro;
        //        encform.Cv = _viewencounterinfo.CV;
        //        encform.Abd = _viewencounterinfo.ABD;
        //        encform.AdminId = _viewencounterinfo.AdminId;
        //        encform.RequestId = _viewencounterinfo.RequestId;
        //        encform.PhysicianId = _viewencounterinfo.PhysicianId;
        //        encform.Temp = _viewencounterinfo.Temp;
        //        encform.BloodPressureDiastolic = _viewencounterinfo.Bpd;
        //        encform.BloodPressureSystolic = _viewencounterinfo.Bp;
        //        encform.Allergies = _viewencounterinfo.Allergies;
        //        encform.Chest = _viewencounterinfo.Chest;
        //        encform.Diagnosis = _viewencounterinfo.Diagnosis;
        //        encform.FollowUp = _viewencounterinfo.Followup;
        //        encform.TreatmentPlan = _viewencounterinfo.Treatment;
        //        encform.Heent = _viewencounterinfo.Heent;
        //        encform.Extremeties = _viewencounterinfo.Extr;
        //        //encform.IsFinalize = _viewencounterinfo.finalizwe;
        //        encform.MedicalHistory = _viewencounterinfo.History;
        //        encform.Medications = _viewencounterinfo.Medications;
        //        encform.MedicationsDispensed = _viewencounterinfo.MDispensed;
        //        encform.O2 = _viewencounterinfo.O2;
        //        encform.Pain = _viewencounterinfo.Pain;
        //        //encform.Physician = _viewencounterinfo.physician;
        //        //encform.Request = _viewencounterinfo.request;
        //        _context.EncounterForms.Update(encform);
        //        _context.SaveChanges();
        //    }
        //}
        public ViewEncounter EncounterInfo(int id)
        {
            var encounter = (from rc in _context.RequestClients
                             join en in _context.EncounterForms on rc.RequestId equals en.RequestId into renGroup
                             from subEn in renGroup.DefaultIfEmpty()
                             where rc.RequestId == id
                             select new ViewEncounter
                             {
                                 RequestId = rc.RequestId,
                                 FirstName = rc.RcFirstName,
                                 LastName = rc.RcLastName,
                                 Location = rc.Address,
                                 DOB = new DateTime((int)rc.IntYear, Convert.ToInt32(rc.StrMonth.Trim()), (int)rc.IntDate),
                                 //DOS = (DateTime)subEn.DateOfService,
                                 Mobile = rc.PhoneNumber,
                                 Email = rc.Email,
                                 Injury = subEn.HistoryOfPresentIllnessOrInjury,
                                 History = subEn.MedicalHistory,
                                 Medications = subEn.Medications,
                                 Allergies = subEn.Allergies,
                                 Temp = subEn.Temp,
                                 HR = subEn.Hr,
                                 RR = subEn.Rr,
                                 Bp = subEn.BloodPressureSystolic,
                                 Bpd = subEn.BloodPressureDiastolic,
                                 O2 = subEn.O2,
                                 Pain = subEn.Pain,
                                 Heent = subEn.Heent,
                                 CV = subEn.Cv,
                                 Chest = subEn.Chest,
                                 ABD = subEn.Abd,
                                 Extr = subEn.Extremeties,
                                 Skin = subEn.Skin,
                                 Neuro = subEn.Neuro,
                                 Other = subEn.Other,
                                 Diagnosis = subEn.Diagnosis,
                                 Treatment = subEn.TreatmentPlan,
                                 MDispensed = subEn.MedicationsDispensed,
                                 Procedures = subEn.Procedures,
                                 Followup = subEn.FollowUp
                             }).FirstOrDefault();
            return encounter;
        }
        public void EditEncounterinfo(ViewEncounter ve)
        {
            var RC = _context.RequestClients.FirstOrDefault(rc => rc.RequestId == ve.RequestId);

            RC.RcFirstName = ve.FirstName;
            RC.RcLastName = ve.LastName;
            RC.Address = ve.Location;
            RC.StrMonth = ve.DOB.Month.ToString();
            RC.IntDate = ve.DOB.Day;
            RC.IntYear = ve.DOB.Year;
            RC.PhoneNumber = ve.Mobile;
            RC.Email = ve.Email;
            _context.Update(RC);
            var E = _context.EncounterForms.FirstOrDefault(e => e.RequestId == ve.RequestId);
            if (E == null)
            {
                E = new EncounterForm { RequestId = (int)ve.RequestId };
                _context.EncounterForms.Add(E);
            }

            E.MedicalHistory = ve.History;
            E.HistoryOfPresentIllnessOrInjury = ve.Injury;
            E.Medications = ve.Medications;
            E.Allergies = ve.Allergies;
            E.Temp = ve.Temp;
            E.Hr = ve.HR;
            E.Rr = ve.RR;
            E.BloodPressureSystolic = ve.Bp;
            E.BloodPressureDiastolic = ve.Bpd;
            E.O2 = ve.O2;
            E.Pain = ve.Pain;
            E.Heent = ve.Heent;
            E.Cv = ve.CV;
            E.Chest = ve.Chest;
            E.Abd = ve.ABD;
            E.Extremeties = ve.Extr;
            E.Skin = ve.Skin;
            E.Neuro = ve.Neuro;
            E.Other = ve.Other;
            E.Diagnosis = ve.Diagnosis;
            E.TreatmentPlan = ve.Treatment;
            E.MedicationsDispensed = ve.MDispensed;
            E.Procedures = ve.Procedures;
            E.FollowUp = ve.Followup;
            _context.SaveChanges();


        }

    }
}

