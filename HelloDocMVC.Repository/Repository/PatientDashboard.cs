using HelloDocMVC.Entity.DataContext;
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
    public class PatientDashboard : IPatientDashboard
    {
        private readonly HelloDocDbContext _context;
        
        public PatientDashboard(HelloDocDbContext context)

        {
            _context = context;
          
        }
        public List<PatientDashboardList> PatientDashboardList(int id)
        {
            var items = _context.Requests.Include(x => x.RequestWiseFiles).Where(x => x.UserId == id).Select(x => new PatientDashboardList
            {
                createdDate = x.CreatedDate,
                Status = (Status)x.Status,
                RequestId = x.RequestId,
                Fcount = x.RequestWiseFiles.Count()

            }).ToList();
            return items;
        }
    }
}
