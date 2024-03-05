using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Entity.Models.ViewModel;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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
        public List<ViewDocument> Document(int? requestid)
        {
            var items = _context.RequestWiseFiles.Include(m => m.Request)
                .Where(x => x.RequestId == requestid).Select(m => new ViewDocument
                {
                    CreatedDate = m.CreatedDate,
                    RFirstName = m.Request.RFirstName,
                    FileName = m.FileName
                }).ToList();
            return items;
        }
        public void UploadDoc(int RequestId, IFormFile? UploadFile)
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
                    RequestId = RequestId,
                    FileName = UploadFile.FileName,
                    CreatedDate = DateTime.Now,
                };
                _context.RequestWiseFiles.Add(requestwisefile);
                _context.SaveChanges();
            }
        }
    }
}
