using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models.ViewModel;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
    }
}
