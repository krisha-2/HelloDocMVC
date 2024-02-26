
using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Repository.Repository.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business = HelloDocMVC.Entity.Models.Business;
using Concierge = HelloDocMVC.Entity.Models.Concierge;

namespace HelloDocMVC.Repository.Repository
{
    public class PatientForms: IPatientForms
    {
        private readonly HelloDocDbContext _context;

        public PatientForms(HelloDocDbContext context)
        {
            _context = context;
        }

        public void PatientRequest(Patient viewdata)
        {
            AspNetUser A = new AspNetUser();
            User U = new User();
            Request R = new Request();
            RequestClient RClient = new RequestClient();
            var isexist = _context.Users.FirstOrDefault(x => x.Email == viewdata.Email);

            //AspNetUser
            if (isexist == null)
            {
                Guid g = Guid.NewGuid();
                A.Id = g.ToString();
                A.UserName = viewdata.FirstName + " " + viewdata.LastName;
                A.Email = viewdata.Email;
                A.PhoneNumber = viewdata.PhoneNumber;
                A.CreatedDate = DateTime.Now;
                _context.Add(A);
                _context.SaveChanges();
                //User
                U.AspNetUserId = A.Id;
                U.FirstName = viewdata.FirstName;
                U.LastName = viewdata.LastName;
                U.CreatedBy = viewdata.FirstName;
                U.Email = viewdata.Email;
                U.Mobile = viewdata.Mobile;
                U.Street = viewdata.Street;
                U.City = viewdata.City;
                U.State = viewdata.State;
                U.ZipCode = viewdata.ZipCode;
                U.Status = 5;
                U.StrMonth = (viewdata.DOB.Month).ToString();
                U.IntDate = viewdata.DOB.Day;
                U.IntYear = viewdata.DOB.Year;
                U.CreatedDate = DateTime.Now;
                _context.Add(U);
                _context.SaveChanges();
            }
            //Request
            R.RequestTypeId = 2;
            if (isexist == null)
            {
                R.UserId = U.UserId;
            }
            else
            {
                R.UserId = isexist.UserId;
            }
            R.RFirstName = viewdata.FirstName;
            R.RLastName = viewdata.LastName;
            R.PhoneNumber = viewdata.PhoneNumber;
            R.Email = viewdata.Email;
            R.Status = 5;
            R.RelationName = viewdata.RelationName;
            R.ConfirmationNumber = R.PhoneNumber;
            R.IsUrgentEmailSent = new BitArray(1);
            R.CreatedDate = DateTime.Now;
            _context.Add(R);
            _context.SaveChanges();
            //RequestClient
            RClient.RequestId = R.RequestId;
            RClient.RcFirstName = viewdata.FirstName;
            RClient.StrMonth = (viewdata.DOB.Month).ToString();
            RClient.IntDate = viewdata.DOB.Day;
            RClient.IntYear = viewdata.DOB.Year;
            RClient.Address = viewdata.Street;
            RClient.City = viewdata.City;
            RClient.State = viewdata.State;
            RClient.Notes = viewdata.Symptoms;
            RClient.ZipCode = viewdata.ZipCode;
            RClient.RcLastName = viewdata.LastName;
            RClient.Email = viewdata.Email;
            RClient.PhoneNumber = viewdata.PhoneNumber;
            RClient.Address = viewdata.Street + "," + viewdata.City + "," + viewdata.State;
            _context.Add(RClient);
            _context.SaveChanges();

            if (viewdata.UploadFile != null)
            {
                string FilePath = "wwwroot\\Uploads";
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileNameWithPath = Path.Combine(path, viewdata.UploadFile.FileName);
                //viewdata.UploadImage = "~" + FilePath.Replace("wwwroot\\", "/") + "/" + viewdata.UploadFile.FileName;

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewdata.UploadFile.CopyTo(stream);
                }

                var RequestWiseFile = new RequestWiseFile
                {
                    RequestId = R.RequestId,
                    FileName = viewdata.UploadFile.FileName,
                    CreatedDate = DateTime.Now,
                };
                _context.RequestWiseFiles.Add(RequestWiseFile);
                _context.SaveChanges();
            }
         
        }

        public void FamilyFriendRequest(FamilyFriend viewdata)
        {
            AspNetUser A = new();
            User U = new();
            Request R = new();
            RequestClient RClient = new();
            var isexist = _context.Users.FirstOrDefault(x => x.Email == viewdata.Email);
            //AspNetUser

            //if (isexist == null)
            //{


            //    Guid g = Guid.NewGuid();
            //    A.Id = g.ToString();
            //    A.UserName = viewdata.FirstName;
            //    A.Email = viewdata.Email;
            //    A.PhoneNumber = viewdata.PhoneNumber;
            //    A.CreatedDate = DateTime.Now;
            //    _context.Add(A);
            //    await _context.SaveChangesAsync();
            //    //User
            //    U.AspNetUserId = A.Id;
            //    U.FirstName = viewdata.FirstName;
            //    U.LastName = viewdata.LastName;
            //    U.CreatedBy = viewdata.FirstName;
            //    U.Email = viewdata.Email;
            //    U.Mobile = viewdata.Mobile;
            //    U.Street = viewdata.Street;
            //    U.City = viewdata.City;
            //    U.State = viewdata.State;
            //    U.ZipCode = viewdata.ZipCode;
            //    U.Status = 5;
            //    U.StrMonth = (viewdata.DOB.Month).ToString();
            //    U.IntDate = viewdata.DOB.Day;
            //    U.IntYear = viewdata.DOB.Year;
            //    U.CreatedDate = DateTime.Now;
            //    _context.Add(U);
            //    await _context.SaveChangesAsync();
            //}
            //Request
            R.RequestTypeId = 3;
            if (isexist == null)
            {
                R.UserId = U.UserId;
            }
            else
            {
                R.UserId = isexist.UserId;
            }
            R.RFirstName = viewdata.First_Name;
            R.RLastName = viewdata.Last_Name;
            R.PhoneNumber = viewdata.Phone_Number;
            R.Email = viewdata.E_mail;
            R.Status = 2;
            R.RelationName = viewdata.RelationName;
            R.ConfirmationNumber = R.PhoneNumber;
            R.IsUrgentEmailSent = new BitArray(1);
            R.CreatedDate = DateTime.Now;
            _context.Add(R);
            _context.SaveChanges();
            //RequestClient
            RClient.RequestId = R.RequestId;
            RClient.RcFirstName = viewdata.FirstName;
            RClient.StrMonth = (viewdata.DOB.Month).ToString();
            RClient.IntDate = viewdata.DOB.Day;
            RClient.IntYear = viewdata.DOB.Year;
            RClient.Address = viewdata.Street;
            RClient.City = viewdata.City;
            RClient.State = viewdata.State;
            RClient.Notes = viewdata.Symptoms;
            RClient.ZipCode = viewdata.ZipCode;
            RClient.RcLastName = viewdata.LastName;
            RClient.Email = viewdata.Email;
            RClient.PhoneNumber = viewdata.PhoneNumber;
            RClient.Address = viewdata.Street + "," + viewdata.City + "," + viewdata.State;
            _context.Add(RClient);
            _context.SaveChanges();
         
        }

        public void ConciergeRequest(Concierge viewdata)
        {
            AspNetUser A = new AspNetUser();
            User U = new User();
            Request R = new Request();
            RequestClient RClient = new RequestClient();
            Entity.DataModels.Concierge C = new Entity.DataModels.Concierge();
            RequestConcierge RConcierge = new RequestConcierge();
            var isexist = _context.Users.FirstOrDefault(x => x.Email == viewdata.Email);
            //AspNetUser
            //Guid g = Guid.NewGuid();
            //A.Id = g.ToString();
            //A.UserName = viewdata.FirstName + "," + viewdata.LastName;
            //A.Email = viewdata.Email;
            //A.PhoneNumber = viewdata.PhoneNumber;
            //A.CreatedDate = DateTime.Now;
            //_context.Add(A);
            //await _context.SaveChangesAsync();
            ////User
            //U.AspNetUserId = A.Id;
            //U.FirstName = viewdata.FirstName;
            //U.LastName = viewdata.LastName;
            //U.CreatedBy = viewdata.FirstName;
            //U.Email = viewdata.Email;
            //U.Mobile = viewdata.Mobile;
            //U.Street = viewdata.Street;
            //U.City = viewdata.City;
            //U.State = viewdata.State;
            //U.ZipCode = viewdata.ZipCode;
            //U.Status = 5;
            //U.StrMonth = (viewdata.DOB.Month).ToString();
            //U.IntDate = viewdata.DOB.Day;
            //U.IntYear = viewdata.DOB.Year;
            //U.CreatedDate = DateTime.Now;
            //_context.Add(U);
            //await _context.SaveChangesAsync();
            //Request
            R.RequestTypeId = 3;
            if (isexist == null)
            {
                R.UserId = U.UserId;
            }
            else
            {
                R.UserId = isexist.UserId;
            }
            R.RFirstName = viewdata.First_Name;
            R.RLastName = viewdata.Last_Name;
            R.PhoneNumber = viewdata.Phone_Number;
            R.Email = viewdata.E_mail;
            R.Status = 5;
            R.RelationName = viewdata.RelationName;
            R.ConfirmationNumber = R.PhoneNumber;
            R.IsUrgentEmailSent = new BitArray(1);
            R.CreatedDate = DateTime.Now;
            _context.Add(R);
            _context.SaveChanges();
            //RequestClient
            RClient.RequestId = R.RequestId;
            RClient.RcFirstName = viewdata.FirstName;
            RClient.StrMonth = (viewdata.DOB.Month).ToString();
            RClient.IntDate = viewdata.DOB.Day;
            RClient.IntYear = viewdata.DOB.Year;
            RClient.Address = viewdata.Street;
            RClient.City = viewdata.City;
            RClient.State = viewdata.State;
            RClient.Notes = viewdata.Symptoms;
            RClient.ZipCode = viewdata.ZipCode;
            RClient.RcLastName = viewdata.LastName;
            RClient.Email = viewdata.Email;
            RClient.PhoneNumber = viewdata.PhoneNumber;
            RClient.Address = viewdata.Street + "," + viewdata.City + "," + viewdata.State;
            _context.Add(RClient);
            _context.SaveChanges();
            //Concierge

            C.ConciergeName = viewdata.FirstName + "," + viewdata.LastName;
            C.Street = viewdata.Street;
            C.City = viewdata.City;
            C.State = viewdata.State;
            C.ZipCode = viewdata.ZipCode;
            C.CreatedDate = DateTime.Now;
            _context.Add(C);
            _context.SaveChanges();
            //RequestConcierge
            RConcierge.RequestId = R.RequestId;
            RConcierge.ConciergeId = C.ConciergeId;
            _context.Add(RConcierge);
            _context.SaveChanges();
    }

        public void BusinessRequest(Business viewdata)
        {
            AspNetUser A = new AspNetUser();
            User U = new User();
            Request R = new Request();
            RequestClient RClient = new RequestClient();
            Entity.DataModels.Business B = new Entity.DataModels.Business();
            RequestBusiness RBusiness = new RequestBusiness();
            var isexist = _context.Users.FirstOrDefault(x => x.Email == viewdata.Email);
            //AspNetUser
            //Guid g = Guid.NewGuid();
            //A.Id = g.ToString();
            //A.UserName = viewdata.FirstName + "," + viewdata.LastName;
            //A.Email = viewdata.Email;
            //A.PhoneNumber = viewdata.PhoneNumber;
            //A.CreatedDate = DateTime.Now;
            //_context.Add(A);
            //await _context.SaveChangesAsync();
            ////User
            //U.AspNetUserId = A.Id;
            //U.FirstName = viewdata.FirstName;
            //U.LastName = viewdata.LastName;
            //U.CreatedBy = viewdata.FirstName;
            //U.Email = viewdata.Email;
            //U.Mobile = viewdata.Mobile;
            //U.Street = viewdata.Street;
            //U.City = viewdata.City;
            //U.State = viewdata.State;
            //U.ZipCode = viewdata.ZipCode;
            //U.Status = 5;
            //U.StrMonth = (viewdata.DOB.Month).ToString();
            //U.IntDate = viewdata.DOB.Day;
            //U.IntYear = viewdata.DOB.Year;
            //U.CreatedDate = DateTime.Now;
            //_context.Add(U);
            //await _context.SaveChangesAsync();
            //Request
            R.RequestTypeId = 3;
            if (isexist == null)
            {
                R.UserId = U.UserId;
            }
            else
            {
                R.UserId = isexist.UserId;
            }
            R.RFirstName = viewdata.First_Name;
            R.RLastName = viewdata.Last_Name;
            R.PhoneNumber = viewdata.Phone_Number;
            R.Email = viewdata.E_mail;
            R.Status = 5;
            R.RelationName = viewdata.RelationName;
            R.ConfirmationNumber = R.PhoneNumber;
            R.IsUrgentEmailSent = new BitArray(1);
            R.CreatedDate = DateTime.Now;
            _context.Add(R);
            _context.SaveChanges();
            //RequestClient
            RClient.RequestId = R.RequestId;
            RClient.RcFirstName = viewdata.FirstName;
            RClient.StrMonth = (viewdata.DOB.Month).ToString();
            RClient.IntDate = viewdata.DOB.Day;
            RClient.IntYear = viewdata.DOB.Year;
            RClient.Address = viewdata.Street;
            RClient.City = viewdata.City;
            RClient.State = viewdata.State;
            RClient.Notes = viewdata.Symptoms;
            RClient.ZipCode = viewdata.ZipCode;
            RClient.RcLastName = viewdata.LastName;
            RClient.Email = viewdata.Email;
            RClient.PhoneNumber = viewdata.PhoneNumber;
            RClient.Address = viewdata.Street + "," + viewdata.City + "," + viewdata.State;
            _context.Add(RClient);
            _context.SaveChanges();
            //Business
            B.Name = viewdata.FirstName + "," + viewdata.LastName;
            B.Address1 = viewdata.Street;
            B.City = viewdata.City;
            B.Address2 = viewdata.State;
            B.ZipCode = viewdata.ZipCode;
            B.CreatedDate = DateTime.Now;
            _context.Add(B);
            _context.SaveChanges();
            //RequestConcierge
            RBusiness.RequestId = R.RequestId;
            RBusiness.BusinessId = B.BusinessId;
            _context.Add(RBusiness);
            _context.SaveChanges();
    }
    }
    
}

