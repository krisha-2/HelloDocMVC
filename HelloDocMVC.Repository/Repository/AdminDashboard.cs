using HelloDocMVC.Entity.Models;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Collections;
using static HelloDocMVC.Entity.Models.ViewDocuments;
using Org.BouncyCastle.Ocsp;
using System.Linq.Expressions;

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
                                               // orderby req.CreatedDate descending
                                                select new AdminDashboardList
                                                {
                                                    RequestId = req.RequestId,
                                                    RequestTypeId = req.RequestTypeId,
                                                    Requestor = req.RFirstName + " " + req.RLastName,
                                                    PatientName = rc.RcFirstName + " " + rc.RcLastName,
                                                    IntYear = rc.IntYear,
                                                    StrMonth = rc.StrMonth,
                                                    IntDate = rc.IntDate,
                                                    RequestedDate = req.CreatedDate,
                                                    Email = rc.Email,
                                                    RegionId = rg.Name,
                                                    ProviderName = p.FirstName + " " + p.LastName,
                                                    PatientPhoneNumber = rc.PhoneNumber,
                                                    Address = rc.Address + "," + rc.Street + "," + rc.City + "," + rc.State + "," + rc.ZipCode,
                                                    Notes = rc.Notes,
                                                    RequestorPhoneNumber = req.PhoneNumber
                                                }).ToList();
            if (data.IsAscending == true)
            {
                allData = data.SortedColumn switch
                {
                    "PatientName" => allData.OrderBy(x => x.PatientName).ToList(),
                    "Requestor" => allData.OrderBy(x => x.Requestor).ToList(),
                    "Dob" => allData.OrderBy(x => x.DateOfBirth).ToList(),
                    "Address" => allData.OrderBy(x => x.Address).ToList(),
                    "RequestedDate" => allData.OrderBy(x => x.RequestedDate).ToList(),
                    _ => allData.OrderBy(x => x.RequestedDate).ToList()
                };
            }
            else
            {
                allData = data.SortedColumn switch
                {
                    "PatientName" => allData.OrderByDescending(x => x.PatientName).ToList(),
                    "Requestor" => allData.OrderByDescending(x => x.Requestor).ToList(),
                    "Dob" => allData.OrderByDescending(x => x.DateOfBirth).ToList(),
                    "Address" => allData.OrderByDescending(x => x.Address).ToList(),
                    "RequestedDate" => allData.OrderByDescending(x => x.RequestedDate).ToList(),
                    _ => allData.OrderByDescending(x => x.RequestedDate).ToList()
                };
            }
            int totalItemCount = allData.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)data.PageSize);
            List<AdminDashboardList> list1 = allData.Skip((data.CurrentPage - 1) * data.PageSize).Take(data.PageSize).ToList();
            
            PaginatedViewModel paginatedViewModel = new PaginatedViewModel
            {
                adl = list1,
                CurrentPage = data.CurrentPage,
                TotalPages = totalPages,
                PageSize = data.PageSize,
                SearchInput = data.SearchInput,
                SortedColumn = data.SortedColumn,
                IsAscending = data.IsAscending
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
        public async Task<ViewDocuments> GetDocumentByRequest(ViewDocuments viewDocument,int? id)
        {
            var req = _context.Requests.FirstOrDefault(r => r.RequestId == id);
            //ViewDocuments doc = new ViewDocuments();
            //doc.ConfirmationNumber = req.ConfirmationNumber;
            //doc.Firstname = req.RFirstName;
            //doc.Lastname = req.RLastName;
            //doc.RequestID = req.RequestId;
            var result = (from requestWiseFile in _context.RequestWiseFiles
                         join request in _context.Requests on requestWiseFile.RequestId equals request.RequestId
                         join physician in _context.Physicians on request.PhysicianId equals physician.PhysicianId into physicianGroup
                         from phys in physicianGroup.DefaultIfEmpty()
                         join admin in _context.Admins on requestWiseFile.AdminId equals admin.AdminId into adminGroup
                         from adm in adminGroup.DefaultIfEmpty()
                         where request.RequestId == id && requestWiseFile.IsDeleted == new BitArray(1)
                         select new Documents
                         {
                             Uploader = requestWiseFile.PhysicianId != null ? phys.FirstName : (requestWiseFile.AdminId != null ? adm.FirstName : request.RFirstName),
                             isDeleted = requestWiseFile.IsDeleted.ToString(),
                             RequestwisefilesId = requestWiseFile.RequestWiseFileId,
                             Status = requestWiseFile.DocType,
                             Createddate = requestWiseFile.CreatedDate,
                             Filename = requestWiseFile.FileName
                         }).ToList();
            //List<Documents> doclist = new List<Documents>();
            //foreach (var item in result)
            //{
            //    doclist.Add(new Documents
            //    {
            //        Uploader = item.Uploader,
            //        isDeleted = item.isDeleted,
            //        RequestwisefilesId = item.RequestwisefilesId,
            //        Status = item.Status,
            //        Createddate = item.Createddate,
            //        Filename = item.Filename
            //    });
            //}
            //doc.documentslist = doclist;
            //return doc;
            if (viewDocument.IsAscending == true)
            {
                result = viewDocument.SortedColumn switch
                {
                    "Createddate" => result.OrderBy(x => x.Createddate).ToList(),
                    "Uploader" => result.OrderBy(x => x.Uploader).ToList(),
                    "Filename" => result.OrderBy(x => x.Filename).ToList(),
                    _ => result.OrderBy(x => x.Createddate).ToList()
                };
            }
            else
            {
                result = viewDocument.SortedColumn switch
                {
                    "Createddate" => result.OrderByDescending(x => x.Createddate).ToList(),
                    "Uploader" => result.OrderByDescending(x => x.Uploader).ToList(),
                    "Filename" => result.OrderByDescending(x => x.Filename).ToList(),
                    _ => result.OrderByDescending(x => x.Createddate).ToList()
                };
            }
            int totalItemCount = result.Count();
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)viewDocument.PageSize);
            List<Documents> list1 = result.Skip((viewDocument.CurrentPage - 1) * viewDocument.PageSize).Take(viewDocument.PageSize).ToList();
            ViewDocuments vd = new()
            {
                documentslist = list1,
                CurrentPage = viewDocument.CurrentPage,
                TotalPages = totalPages,
                PageSize = viewDocument.PageSize,
                IsAscending = viewDocument.IsAscending,
                SortedColumn = viewDocument.SortedColumn,
                Firstname = req.RFirstName,
                Lastname = req.RLastName,
                //ConfirmationNumber = req.City.Substring(0, 2) + req.IntDate.ToString() + req.StrMonth + req.IntYear.ToString() + req.LastName.Substring(0, 2) + req.FirstName.Substring(0, 2) + "002",
                RequestID = req.RequestId
            };
            return vd;
    }
        public bool SaveDoc(int Requestid, IFormFile file)
        {
            string UploadDoc = FileSave.UploadDoc(file, Requestid);
            var requestwisefile = new RequestWiseFile
            {
                RequestId = Requestid,
                FileName = UploadDoc,
                CreatedDate = DateTime.Now,
                IsDeleted = new BitArray(1),
                AdminId = 1
            };
            _context.RequestWiseFiles.Add(requestwisefile);
            _context.SaveChanges();
            return true;
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
        public bool SendOrder(Orders data)
        {
            try
            {
                OrderDetail od = new()
                {
                    RequestId = data.RequestId,
                    VendorId = data.VendorId,
                    FaxNumber = data.FaxNumber,
                    Email = data.Email,
                    BusinessContact = data.Business_contact,
                    Prescription = data.Prescription,
                    NoOfRefill = data.NoOfRefill,
                    CreatedDate = DateTime.Now,
                    CreatedBy = "65e196bf-b39d-48e8-a3da-ebd3b699dede"
                };
                _context.OrderDetails.Add(od);
                _context.SaveChanges(true);
                var req = _context.Requests.FirstOrDefault(e => e.RequestId == data.RequestId);
                _emailConfig.SendMail(data.Email, "Order Details", "<p>Prescription:" + data.Prescription + "<br> No of Refills: " + data.NoOfRefill + "</p>");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool SendAgreement(int requestid)
        {
            var res = _context.RequestClients.FirstOrDefault(e => e.RequestId == requestid);
            var agreementUrl = "https://localhost:44338/AgreementPage?RequestID=" + requestid;
            _emailConfig.SendMail(res.Email, "Agreement for your request", $"<a href='{agreementUrl}'>Agree/Disagree</a>");
            return true;
        }
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
            alldata.RC_FirstName = reqcl.RcFirstName;
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
        public List<AdminDashboardList> Export(string status)
        {
            List<int> statusdata = status.Split(',').Select(int.Parse).ToList();
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
                                                where statusdata.Contains((int)req.Status)
                                                orderby req.CreatedDate descending
                                                select new AdminDashboardList
                                                {
                                                    RequestId = req.RequestId,
                                                    RequestTypeId = req.RequestTypeId,
                                                    Requestor = req.RFirstName + " " + req.RLastName,
                                                    PatientName = rc.RcFirstName + " " + rc.RcLastName,
                                                    IntYear = rc.IntYear,
                                                    StrMonth = rc.StrMonth,
                                                    IntDate = rc.IntDate,
                                                    RequestedDate = req.CreatedDate,
                                                    Email = rc.Email,
                                                    RegionId = rg.Name,
                                                    ProviderName = p.FirstName + " " + p.LastName,
                                                    PatientPhoneNumber = rc.PhoneNumber,
                                                    Address = rc.Address + "," + rc.Street + "," + rc.City + "," + rc.State + "," + rc.ZipCode,
                                                    Notes = rc.Notes,
                                                    RequestorPhoneNumber = req.PhoneNumber
                                                }).ToList();
            return allData;
        }
        public Boolean SendLink(string FirstName, string LastName, string Email)
        {
            var agreementUrl = "https://localhost:44348/Forms/Index?Name=" + FirstName + " " + LastName + "&Email=" + Email;
            _emailConfig.SendMail(Email, "Link to Request", $"<a href='{agreementUrl}'>Request Page Link</a>");
            return true;
        }
        public void CreateRequest(Patient vdcp)
        {
            AspNetUser A = new();
            User U = new();
            Entity.DataModels.Request R = new();
            RequestClient RC = new();
            var isexist = _context.Users.FirstOrDefault(x => x.Email == vdcp.Email);
            if (isexist == null)
            {
                //AspNetUser Table
                Guid g = Guid.NewGuid();
                A.Id = g.ToString();
                A.UserName = vdcp.UserName;
                A.PasswordHash = vdcp.PassWord;
                A.Email = vdcp.Email;
                A.PhoneNumber = vdcp.Mobile;
                A.CreatedDate = DateTime.Now;
                _context.Add(A);
                _context.SaveChanges();
                //User Table
                U.AspNetUserId = A.Id;
                U.FirstName = vdcp.FirstName;
                U.LastName = vdcp.LastName;
                U.Email = vdcp.Email;
                U.Mobile = vdcp.Mobile;
                U.Street = vdcp.Street;
                U.City = vdcp.City;
                U.State = vdcp.State;
                U.ZipCode = vdcp.ZipCode;
                U.StrMonth = (vdcp.DOB.Month).ToString();
                U.IntDate = vdcp.DOB.Day;
                U.IntYear = vdcp.DOB.Year;
                U.CreatedBy = A.Id;
                U.CreatedDate = DateTime.Now;
                _context.Add(U);
                _context.SaveChanges();
            }

            if (isexist == null)
            {
                R.UserId = U.UserId;
            }
            else
            {
                R.UserId = isexist.UserId;
            }
            U.Status = 1;
            //Request Table
            R.RequestTypeId = 2; // 2 stands for patient in RequestType table
            R.CreatedDate = DateTime.Now;
            R.RFirstName = vdcp.FirstName;
            R.RLastName = vdcp.LastName;
            R.Email = vdcp.Email;
            R.PhoneNumber = vdcp.Mobile;
            R.Status = (short)U.Status;
            R.ConfirmationNumber = R.PhoneNumber;
            R.CreatedUserId = U.UserId;
            _context.Add(R);
            _context.SaveChanges();
            //RequestClient Table
            RC.RequestId = R.RequestId;
            RC.RcFirstName = vdcp.FirstName;
            RC.RcLastName = vdcp.LastName;
            RC.PhoneNumber = vdcp.Mobile;
            RC.Address = vdcp.Street + ", " + vdcp.City + ", " + vdcp.State + ", " + vdcp.ZipCode;
            RC.NotiMobile = R.PhoneNumber;
            RC.NotiEmail = R.Email;
            RC.Notes = vdcp.Symptoms;
            RC.Email = vdcp.Email;
            RC.StrMonth = (vdcp.DOB.Month).ToString();
            RC.IntDate = vdcp.DOB.Day;
            RC.IntYear = vdcp.DOB.Year;
            RC.Street = vdcp.Street;
            RC.City = vdcp.City;
            RC.State = vdcp.State;
            RC.ZipCode = vdcp.ZipCode;
            _context.Add(RC);
            _context.SaveChanges();
        }
        public async Task<bool> SendFileEmail(string ids, int Requestid, string email)
        {
            var v = await GetRequestDetails(Requestid);
            List<int> priceList = ids.Split(',').Select(int.Parse).ToList();
            List<string> files = new();
            foreach (int price in priceList)
            {
                if (price > 0)
                {
                    var data = await _context.RequestWiseFiles.Where(e => e.RequestWiseFileId == price).FirstOrDefaultAsync();
                    files.Add(Directory.GetCurrentDirectory() + "\\wwwroot\\Upload" + data.FileName.Replace("Upload/", "").Replace("/", "\\"));
                }
            }

            if (await _emailConfig.SendMailAsync(email, "All Document Of Your Request " + v.PatientName, "Heeyy " + v.PatientName + " Kindly Check your Attachments", files))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public async Task<ViewActions> GetRequestDetails(int? id)
        {

            return await (from req in _context.Requests
                          join reqClient in _context.RequestClients
                          on req.RequestId equals reqClient.RequestId into reqClientGroup
                          from rc in reqClientGroup.DefaultIfEmpty()
                          join phys in _context.Physicians
                        on req.PhysicianId equals phys.PhysicianId into physGroup
                          from p in physGroup.DefaultIfEmpty()
                          where req.RequestId == id
                          select new ViewActions
                          {
                              PhoneNumber = rc.PhoneNumber,
                              ProviderId = p.PhysicianId,
                              PatientName = rc.RcFirstName + rc.RcLastName,
                              RequestID = req.RequestId,
                              Email = rc.Email

                          }).FirstAsync();
        }
        
    }
}


