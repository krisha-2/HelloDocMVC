using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Entity.Models.ViewModel;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HelloDocMVC.Entity.Models.Constant;

namespace HelloDocMVC.Repository.Repository
{
    public class Records : IRecords
    {
        private readonly HelloDocDbContext _context;
        public Records(HelloDocDbContext context)
        {
            _context = context;
        }
        public List<User> PatientHistory(string fname, string lname, string email, string phone)
        {
            var His = _context.Users
                     .Where(pp => (string.IsNullOrEmpty(fname) || pp.FirstName.Contains(fname))
                               && (string.IsNullOrEmpty(lname) || pp.LastName.Contains(lname))
                               && (string.IsNullOrEmpty(email) || pp.Email.Contains(email))
                               && (string.IsNullOrEmpty(phone) || pp.Mobile.Contains(phone)))
                     .ToList();

            return His;
        }
        public List<PatientDashboardList> RecordsPatientExplore(int UserId)
        {
            List<PatientDashboardList> allData = (from req in _context.Requests
                                             join reqClient in _context.RequestClients
                                             on req.RequestId equals reqClient.RequestId into reqClientGroup
                                             from rc in reqClientGroup.DefaultIfEmpty()
                                             join phys in _context.Physicians
                                             on req.PhysicianId equals phys.PhysicianId into physGroup
                                             from p in physGroup.DefaultIfEmpty()
                                             where req.UserId == UserId
                                             select new PatientDashboardList
                                             {
                                                 PatientName = rc.RcFirstName + " " + rc.RcLastName,
                                                 RequestedDate = (req.CreatedDate),
                                                 Confirmation = req.ConfirmationNumber,
                                                 Physician = p.FirstName + " " + p.LastName,
                                                 ConcludedDate = req.CreatedDate,
                                                 Status = (Status)req.Status,
                                                 RequestTypeId = req.RequestTypeId,
                                                 RequestId = req.RequestId
                                             }).ToList();
            return allData;
        }
        public RecordsData GetFilteredSearchRecords(RecordsData rm)
        {
            List<SearchRecords> allData = (from req in _context.Requests
                                           join reqClient in _context.RequestClients
                                           on req.RequestId equals reqClient.RequestId into reqClientGroup
                                           from rc in reqClientGroup.DefaultIfEmpty()
                                           join phys in _context.Physicians
                                           on req.PhysicianId equals phys.PhysicianId into physGroup
                                           from p in physGroup.DefaultIfEmpty()
                                           join reg in _context.Regions
                                           on rc.RegionId equals reg.RegionId into RegGroup
                                           from rg in RegGroup.DefaultIfEmpty()
                                           join nts in _context.RequestNotes
                                           on req.RequestId equals nts.RequestId into ntsgrp
                                           from nt in ntsgrp.DefaultIfEmpty()
                                           where (req.IsDeleted == new BitArray(1) && rm.Status == 0 || req.Status == rm.Status) &&
                                                 (rm.RequestType == 0 || req.RequestTypeId == rm.RequestType) &&
                                                 (!rm.StartDate.HasValue || req.CreatedDate.Date >= rm.StartDate.Value.Date) &&
                                                 (!rm.EndDate.HasValue || req.CreatedDate.Date <= rm.EndDate.Value.Date) &&
                                                 (rm.PatientName.IsNullOrEmpty() || (req.RFirstName + " " + req.RLastName).ToLower().Contains(rm.PatientName.ToLower())) &&
                                                 (rm.PhysicianName.IsNullOrEmpty() || (p.FirstName + " " + p.LastName).ToLower().Contains(rm.PhysicianName.ToLower())) &&
                                                 (rm.Email.IsNullOrEmpty() || rc.Email.ToLower().Contains(rm.Email.ToLower())) &&
                                                 (rm.PhoneNumber.IsNullOrEmpty() || rc.PhoneNumber.ToLower().Contains(rm.PhoneNumber.ToLower()))
                                           orderby req.CreatedDate
                                           select new SearchRecords
                                           {
                                               ModifiedDate = req.ModifiedDate,
                                               PatientName = req.RFirstName + " " + req.RLastName,
                                               RequestId = req.RequestId,
                                               DateOfService = req.CreatedDate,
                                               PhoneNumber = rc.PhoneNumber ?? "",
                                               Email = rc.Email ?? "",
                                               Address = rc.Address ?? "",
                                               Zip = rc.ZipCode ?? "",
                                               RequestTypeId = req.RequestTypeId,
                                               Status = req.Status,
                                               PhysicianName = p.FirstName + " " + p.LastName ?? "",
                                               AdminNote = nt != null ? nt.AdminNotes ?? "" : "",
                                               PhysicianNote = nt != null ? nt.PhysicianNotes ?? "" : "",
                                               PatientNote = rc.Notes ?? ""
                                           }).ToList();

            int totalItemCount = allData.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)rm.PageSize);
            List<SearchRecords> list = allData.Skip((rm.CurrentPage - 1) * rm.PageSize).Take(rm.PageSize).ToList();

            RecordsData records = new()
            {
                SearchRecords = list,
                CurrentPage = rm.CurrentPage,
                TotalPages = totalPages,
                PageSize = rm.PageSize,
            };

            for (int i = 0; i < records.SearchRecords.Count; i++)
            {
                if (records.SearchRecords[i].Status == 9)
                {
                    records.SearchRecords[i].CloseCaseDate = records.SearchRecords[i].ModifiedDate;
                }
                else
                {
                    records.SearchRecords[i].CloseCaseDate = null;
                }
                if (records.SearchRecords[i].Status == 3 && records.SearchRecords[i].PhysicianName != null)
                {
                    var data = _context.RequestStatusLogs.FirstOrDefault(x => (x.Status == 3) && (x.RequestId == records.SearchRecords[i].RequestId));
                    //records.SearchRecords[i].CancelByProviderNote = data.Notes;
                }

            }
            return records;
        }
        public bool DeleteRequest(int? RequestId)
        {
            try
            {
                var data = _context.Requests.FirstOrDefault(v => v.RequestId == RequestId);
                data.IsDeleted = new BitArray(1);
                data.IsDeleted[0] = true;
                data.ModifiedDate = DateTime.Now;
                _context.Requests.Update(data);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public RecordsData BlockHistory(RecordsData rm)
        {
            List<BlockRequests> data = (from req in _context.BlockRequests
                                        where (!rm.StartDate.HasValue || req.CreatedDate.Value.Date == rm.StartDate.Value.Date) &&
                                              (rm.PatientName.IsNullOrEmpty() || _context.Requests.FirstOrDefault(e => e.RequestId == Convert.ToInt32(req.RequestId)).RFirstName.ToLower().Contains(rm.PatientName.ToLower())) &&
                                              (rm.Email.IsNullOrEmpty() || req.Email.ToLower().Contains(rm.Email.ToLower())) &&
                                              (rm.PhoneNumber.IsNullOrEmpty() || req.PhoneNumber.ToLower().Contains(rm.PhoneNumber.ToLower()))
                                        select new BlockRequests
                                        {
                                            PatientName = _context.Requests.FirstOrDefault(e => e.RequestId == Convert.ToInt32(req.RequestId)).RFirstName,
                                            Email = req.Email,
                                            CreatedDate = (DateTime)req.CreatedDate,
                                            IsActive = req.IsActive,
                                            RequestId = Convert.ToInt32(req.RequestId),
                                            PhoneNumber = req.PhoneNumber,
                                            Reason = req.Reason
                                        }).ToList();

            int totalItemCount = data.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)rm.PageSize);
            List<BlockRequests> list = data.Skip((rm.CurrentPage - 1) * rm.PageSize).Take(rm.PageSize).ToList();

            RecordsData model = new()
            {
                BlockRequests = list,
                CurrentPage = rm.CurrentPage,
                TotalPages = totalPages,
                PageSize = rm.PageSize,
            };

            return model;
        }
        public bool Unblock(int RequestId, string id)
        {
            try
            {
                BlockRequest block = _context.BlockRequests.FirstOrDefault(e => e.RequestId == RequestId);
                block.IsActive = new BitArray(1);
                block.IsActive[0] = true;
                _context.BlockRequests.Update(block);
                _context.SaveChanges();

                Request request = _context.Requests.FirstOrDefault(e => e.RequestId == RequestId);
                request.Status = 1;
                request.ModifiedDate = DateTime.Now;
                _context.Requests.Update(request);
                _context.SaveChanges();

                var admindata = _context.Admins.FirstOrDefault(e => e.AspNetUserId == id);
                RequestStatusLog rs = new()
                {
                    Status = 1,
                    RequestId = RequestId,
                    AdminId = admindata.AdminId,
                    CreatedDate = DateTime.Now
                };
                _context.RequestStatusLogs.Add(rs);
                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #region GetFilteredEmailLogs
        public RecordsData GetFilteredEmailLogs(RecordsData rm)
        {
            List<EmailLogs> allData = (from req in _context.EmailLogs
                                       where (rm.AccountType == null || rm.AccountType == 0 || req.RoleId == rm.AccountType) &&
                                             (!rm.StartDate.HasValue || req.CreateDate.Date == rm.StartDate.Value.Date) &&
                                             (!rm.EndDate.HasValue || req.SentDate.Value.Date == rm.EndDate.Value.Date) &&
                                             (rm.ReceiverName.IsNullOrEmpty() || _context.AspNetUsers.FirstOrDefault(e => e.Email == req.EmailId).UserName.ToLower().Contains(rm.ReceiverName.ToLower())) &&
                                             (rm.Email.IsNullOrEmpty() || req.EmailId.ToLower().Contains(rm.Email.ToLower()))
                                       select new EmailLogs
                                       {
                                           Recipient = _context.AspNetUsers.FirstOrDefault(e => e.Email == req.EmailId).UserName ?? null,
                                           ConfirmationNumber = req.ConfirmationNumber,
                                           CreateDate = req.CreateDate,
                                           EmailTemplate = req.EmailTemplate,
                                           FilePath = req.FilePath,
                                           SentDate = (DateTime)req.SentDate,
                                           RoleId = req.RoleId,
                                           EmailId = req.EmailId,
                                           SentTries = req.SentTries,
                                           SubjectName = req.SubjectName,
                                           Action = (int)req.Action
                                       }).ToList();

            int totalItemCount = allData.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)rm.PageSize);
            List<EmailLogs> list = allData.Skip((rm.CurrentPage - 1) * rm.PageSize).Take(rm.PageSize).ToList();

            RecordsData records = new()
            {
                EmailLogs = list,
                CurrentPage = rm.CurrentPage,
                TotalPages = totalPages,
                PageSize = rm.PageSize
            };

            return records;
        }
        #endregion GetFilteredEmailLogs

        #region GetFilteredSMSLogs
        public RecordsData GetFilteredSMSLogs(RecordsData rm)
        {
            List<SMSLogs> allData = (from req in _context.Smslogs
                                     where (rm.AccountType == null || rm.AccountType == 0 || req.RoleId == rm.AccountType) &&
                                           (!rm.StartDate.HasValue || req.CreateDate.Date == rm.StartDate.Value.Date) &&
                                           (!rm.EndDate.HasValue || req.SentDate.Value.Date == rm.EndDate.Value.Date) &&
                                           (rm.ReceiverName.IsNullOrEmpty() || _context.AspNetUsers.FirstOrDefault(e => e.PhoneNumber == req.MobileNumber).UserName.ToLower().Contains(rm.ReceiverName.ToLower())) &&
                                           (rm.PhoneNumber.IsNullOrEmpty() || req.MobileNumber.ToLower().Contains(rm.PhoneNumber.ToLower()))
                                     select new SMSLogs
                                     {
                                         Recipient = _context.AspNetUsers.FirstOrDefault(e => e.PhoneNumber == req.MobileNumber).UserName,
                                         ConfirmatioNumber = req.ConfirmationNumber,
                                         CreateDate = req.CreateDate,
                                         SmsTemplate = req.Smstemplate,
                                         SentDate = (DateTime)req.SentDate,
                                         RoleId = req.RoleId,
                                         MobileNumber = req.MobileNumber,
                                         SentTries = req.SentTries,
                                         Action = req.Action
                                     }).ToList();

            int totalItemCount = allData.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)rm.PageSize);
            List<SMSLogs> list = allData.Skip((rm.CurrentPage - 1) * rm.PageSize).Take(rm.PageSize).ToList();

            RecordsData records = new()
            {
                SMSLogs = list,
                CurrentPage = rm.CurrentPage,
                TotalPages = totalPages,
                PageSize = rm.PageSize
            };
            return records;
        }
        #endregion GetFilteredSMSLogs
    }
}
