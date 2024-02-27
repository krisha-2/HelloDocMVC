using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using HelloDocMVC.Entity.DataModels;
using System.Net;
using static HelloDocMVC.Entity.Models.Constant;
using System.Reflection;

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
                           //DateOfBirth = new DateTime((int)req.Requestclient.Intyear, Convert.ToInt32(req.Requestclient.Strmonth.Trim()), (int)req.Requestclient.Intdate),
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
            ViewCase viewCase = _context.Requests.Join
                       (_context.RequestClients,
                       requestclients => requestclients.RequestId, requests => requests.RequestId,
                       (requests, requestclients) => new { Request = requests, Requestclient = requestclients }
                       )
                       .Where(R=>R.Request.RequestId == id)
                       .Select(req => new ViewCase()
                       {
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
                                                    //Dob = new DateOnly((int)rc.Intyear, DateTime.ParseExact(rc.Strmonth, "MMMM", new CultureInfo("en-US")).Month, (int)rc.Intdate),
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
    }
}
