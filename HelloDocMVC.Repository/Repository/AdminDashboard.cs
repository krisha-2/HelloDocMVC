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
                           Address = req.Requestclient.Address + " " + req.Requestclient.Street + " " + req.Requestclient.City + " " + req.Requestclient.State + " " + req.Requestclient.Zipcode
                       })
                       .OrderByDescending(req => req.RequestedDate)
                        .ToList();
            return list;
        }
    }
}
