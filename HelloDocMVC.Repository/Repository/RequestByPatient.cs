using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Repository.Repository
{
    public class RequestByPatient : IRequestByPatient
    {
        private readonly HelloDocDbContext _context;
        public RequestByPatient(HelloDocDbContext context)
        {
            _context = context;
        }
        public Patient viewMeData(int id)
        {
            var ViewPatientCreateRequest = _context.Users
                              .Where(r => (r.UserId) == id)
                              .Select(r => new Patient
                              {
                                  FirstName = r.FirstName,
                                  LastName = r.LastName,
                                  Email = r.Email,
                                  Mobile = r.Mobile,
                                  DOB = new DateTime((int)r.IntYear, Convert.ToInt32(r.StrMonth.Trim()), (int)r.IntDate),
                              })
                              .FirstOrDefault();
            return ViewPatientCreateRequest;
        }
        public void meRequset(Patient viewPatientReq)
        {
            var Request = new Request();
            var Requestclient = new RequestClient();
            var isexist = _context.Users.FirstOrDefault(x => x.Email == viewPatientReq.Email);

            Request.RequestTypeId = 2;
            Request.Status = 1;
            Request.RFirstName = viewPatientReq.FirstName;
            Request.RLastName = viewPatientReq.LastName;
            Request.UserId = isexist.UserId;
            Request.Email = viewPatientReq.Email;
            Request.PhoneNumber = viewPatientReq.Mobile;
            Request.CreatedDate = DateTime.Now;
            Request.IsUrgentEmailSent = new BitArray(1);
            _context.Requests.Add(Request);
            _context.SaveChanges();

            Requestclient.RequestId = Request.RequestId;
            Requestclient.RcFirstName = viewPatientReq.FirstName;
            Requestclient.RcLastName = viewPatientReq.LastName;
            Requestclient.Address = viewPatientReq.Street;
            Requestclient.Email = viewPatientReq.Email;
            Requestclient.PhoneNumber = viewPatientReq.Mobile;
            Requestclient.Notes = viewPatientReq.Symptoms;
            Requestclient.IntDate = viewPatientReq.DOB.Day;
            Requestclient.IntYear = viewPatientReq.DOB.Year;
            Requestclient.StrMonth = (viewPatientReq.DOB.Month).ToString();
            _context.RequestClients.Add(Requestclient);
            _context.SaveChanges();

            if (viewPatientReq.file != null)
            {
                string FilePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                string fileNameWithPath = Path.Combine(path, viewPatientReq.file.FileName);
                //viewPatientReq.UploadImage = "~" + FilePath.Replace("wwwroot\\", "/") + "/" + viewpatientcreaterequest.UploadFile.FileName;

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewPatientReq.file.CopyTo(stream);
                }
                var requestwisefile = new RequestWiseFile
                {
                    RequestId = Request.RequestId,
                    FileName = viewPatientReq.file.FileName,
                    CreatedDate = DateTime.Now,
                };
                _context.RequestWiseFiles.Add(requestwisefile);
                _context.SaveChanges();
            }
        }
        public void elseRequset(FamilyFriend viewFamilyReq,int id)
        {
            var Request = new Request();
            var Requestclient = new RequestClient();
            var isexist = _context.Users.FirstOrDefault(x => x.Email == viewFamilyReq.Email);

            Request.RequestTypeId = 3;
            Request.Status = 1;
            Request.UserId = id;
            Request.RFirstName = viewFamilyReq.FirstName;
            Request.RLastName = viewFamilyReq.LastName;
            Request.Email = viewFamilyReq.Email;
            Request.PhoneNumber = viewFamilyReq.Mobile;
            Request.RelationName = viewFamilyReq.RelationName;
            Request.CreatedDate = DateTime.Now;
            Request.IsUrgentEmailSent = new BitArray(1);
            _context.Requests.Add(Request);
            _context.SaveChanges();

            Requestclient.RequestId = Request.RequestId;
            Requestclient.RcFirstName = viewFamilyReq.FirstName;
            Requestclient.RcLastName = viewFamilyReq.LastName;
            Requestclient.Address = viewFamilyReq.Street;
            Requestclient.Email = viewFamilyReq.Email;
            Requestclient.PhoneNumber = viewFamilyReq.Mobile;
            Requestclient.IntDate = viewFamilyReq.DOB.Day;
            Requestclient.IntYear = viewFamilyReq.DOB.Year;
            Requestclient.StrMonth = (viewFamilyReq.DOB.Month).ToString();
            Requestclient.Notes = viewFamilyReq.Symptoms;
            Requestclient.Address = viewFamilyReq.Street + "," + viewFamilyReq.City + "," + viewFamilyReq.State;
            _context.RequestClients.Add(Requestclient);
            _context.SaveChanges();

            if (viewFamilyReq.file != null)
            {
                string FilePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                string fileNameWithPath = Path.Combine(path, viewFamilyReq.file.FileName);
                //viewPatientReq.UploadImage = "~" + FilePath.Replace("wwwroot\\", "/") + "/" + viewpatientcreaterequest.UploadFile.FileName;

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewFamilyReq.file.CopyTo(stream);
                }
                var requestwisefile = new RequestWiseFile
                {
                    RequestId = Request.RequestId,
                    FileName = viewFamilyReq.file.FileName,
                    CreatedDate = DateTime.Now,
                };
                _context.RequestWiseFiles.Add(requestwisefile);
                _context.SaveChanges();
            }
        }
    }
}
