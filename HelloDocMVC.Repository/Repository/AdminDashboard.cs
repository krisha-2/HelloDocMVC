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
using HelloDocMVC.Entity.DataModels;
using System.Net;
using static HelloDocMVC.Entity.Models.Constant;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using System.Collections;

namespace HelloDocMVC.Repository.Repository
{
    public class AdminDashboard : IAdminDashboard
    {
        private readonly HelloDocDbContext _context;
        public AdminDashboard(HelloDocDbContext context)
        {
            _context = context;
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
                           //DateOfBirth = new DateTime((int)req.Requestclient.IntYear, Convert.ToInt32(req.Requestclient.StrMonth.Trim()), (int)req.Requestclient.IntDate),
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
        public CountStatusWiseRequestModel IndexData()
        {
            return new CountStatusWiseRequestModel
            {
                NewRequest = _context.Requests.Where(r => r.Status == 1).Count(),
                PendingRequest = _context.Requests.Where(r => r.Status == 2).Count(),
                ActiveRequest = _context.Requests.Where(r => (r.Status == 4 || r.Status == 5)).Count(),
                ConcludeRequest = _context.Requests.Where(r => r.Status == 6).Count(),
                ToCloseRequest = _context.Requests.Where(r => (r.Status == 3 || r.Status == 7 || r.Status == 8)).Count(),
                UnpaidRequest = _context.Requests.Where(r => r.Status == 9).Count(),
                adminDashboardList = NewRequestData()
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
                           RequestId= id,
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
        public List<AdminDashboardList> GetRequests(string Status)
        {
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
                                                where statusdata.Contains(req.Status)
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
                                                    Region = rg.Name,
                                                    ProviderName = p.FirstName + " " + p.LastName,
                                                    PhoneNumber = rc.PhoneNumber,
                                                    Address = rc.Address + "," + rc.Street + "," + rc.City + "," + rc.State + "," + rc.ZipCode,
                                                    Notes = rc.Notes,
                                                    //ProviderId = req.PhysicianId,
                                                    RequestorPhoneNumber = req.PhoneNumber
                                                }).ToList();
            return allData;
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
            rsl.PhysicianId = ProviderId;
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
            ViewDocument items = _context.RequestClients.Include(rc => rc.Request)
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
        //public async Task<bool> Business(int RequestId, int ProviderId, string notes)
        //{

        //    var request = await _context.Requests.FirstOrDefaultAsync(req => req.RequestId == RequestId);
        //    request.PhysicianId = ProviderId;
        //    request.Status = 2;
        //    _context.Requests.Update(request);
        //    _context.SaveChanges();

        //    RequestStatusLog rsl = new RequestStatusLog();
        //    rsl.RequestId = RequestId;
        //    rsl.PhysicianId = ProviderId;
        //    rsl.Notes = notes;

        //    rsl.CreatedDate = DateTime.Now;
        //    rsl.Status = 2;
        //    _context.RequestStatusLogs.Update(rsl);
        //    _context.SaveChanges();

        //    return true;
        //}
    }
}

