using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Repository.Repository.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelloDocMVC.Entity.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace HelloDocMVC.Repository.Repository
{
    public class Provider : IProvider
    {
        private readonly HelloDocDbContext _context;
        private readonly EmailConfiguration _emailConfig;
        public Provider(HelloDocDbContext context, EmailConfiguration emailConfig)
        {
            _context = context;
            _emailConfig = emailConfig;
        }
        public async Task<List<ViewProvider>> PhysicianAll()
        {
            List<ViewProvider> data = await (from r in _context.Physicians
                                             join Notifications in _context.PhysicianNotifications
                                             on r.PhysicianId equals Notifications.PhysicianId into aspGroup
                                             from nof in aspGroup.DefaultIfEmpty()
                                             join role in _context.Roles
                                               on r.RoleId equals role.RoleId into roleGroup
                                               from roles in roleGroup.DefaultIfEmpty()
                                               where r.IsDeleted == new BitArray(1)
                                               select new ViewProvider
                                               {
                                                   notificationid = nof.Id,
                                                   Createddate = r.CreatedDate,
                                                   Physicianid = r.PhysicianId,
                                                   Address1 = r.Address1,
                                                   Address2 = r.Address2,
                                                   Adminnotes = r.AdminNotes,
                                                   Altphone = r.AltPhone,
                                                   Businessname = r.BusinessName,
                                                   Businesswebsite = r.BusinessWebsite,
                                                   City = r.City,
                                                   Firstname = r.FirstName,
                                                   Lastname = r.LastName,
                                                   notification = nof.IsNotificationStopped,
                                                   role = roles.Name,
                                                   Status = r.Status,
                                                   Email = r.Email
                                               })
                                        .ToListAsync();
            return data;
        }
        public async Task<List<ViewProvider>> PhysicianByRegion(int? region)
        {
            List<ViewProvider> data = await (
                                        from pr in _context.PhysicianRegions
                                        join ph in _context.Physicians
                                        on pr.PhysicianId equals ph.PhysicianId into rGroup
                                        from r in rGroup.DefaultIfEmpty()
                                        join Notifications in _context.PhysicianNotifications
                                        on r.PhysicianId equals Notifications.PhysicianId into aspGroup
                                        from nof in aspGroup.DefaultIfEmpty()
                                        join role in _context.Roles
                                        on r.RoleId equals role.RoleId into roleGroup
                                        from roles in roleGroup.DefaultIfEmpty()
                                        where pr.RegionId == region
                                        select new ViewProvider
                                        {
                                            Createddate = r.CreatedDate,
                                            Physicianid = r.PhysicianId,
                                            Address1 = r.Address1,
                                            Address2 = r.Address2,
                                            Adminnotes = r.AdminNotes,
                                            Altphone = r.AltPhone,
                                            Businessname = r.BusinessName,
                                            Businesswebsite = r.BusinessWebsite,
                                            City = r.City,
                                            Firstname = r.FirstName,
                                            Lastname = r.LastName,
                                            notification = nof.IsNotificationStopped,
                                            role = roles.Name,
                                            Status = r.Status,
                                            Email = r.Email,
                                            Isnondisclosuredoc = r.IsNonDisclosureDoc == null ? false : true
                                        })
                                        .ToListAsync();
            return data;
        }
        public async Task<bool> ChangeNotificationPhysician(Dictionary<int, bool> changedValuesDict)
        {
            try
            {
                if (changedValuesDict == null)
                {
                    return false;
                }
                else
                {
                    foreach (var item in changedValuesDict)
                    {
                        var ar = _context.PhysicianNotifications.Where(r => r.PhysicianId == item.Key).FirstOrDefault();
                        if (ar != null)
                        {
                            ar.IsNotificationStopped[0] = item.Value;
                            _context.PhysicianNotifications.Update(ar);
                            _context.SaveChanges();
                        }
                        else
                        {
                            PhysicianNotification pn = new PhysicianNotification();
                            pn.PhysicianId = item.Key;
                            pn.IsNotificationStopped = new BitArray(1);
                            pn.IsNotificationStopped[0] = item.Value;
                            _context.PhysicianNotifications.Add(pn);
                            _context.SaveChanges();
                        }
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> PhysicianAddEdit(ViewProvider physiciandata, string AdminId)
        {
            try
            {
                if (physiciandata.UserName != null && physiciandata.PassWord != null)
                {
                    // ASP_User
                    var Aspnetuser = new AspNetUser();
                    var hasher = new PasswordHasher<string>();
                    Aspnetuser.Id = Guid.NewGuid().ToString();
                    Aspnetuser.UserName = physiciandata.UserName;
                    Aspnetuser.PasswordHash = hasher.HashPassword(null, physiciandata.PassWord);
                    Aspnetuser.Email = physiciandata.Email;
                    Aspnetuser.CreatedDate = DateTime.Now;
                    _context.AspNetUsers.Add(Aspnetuser);
                    _context.SaveChanges();

                    //aspnet_user_roles
                    var aspnetuserroles = new AspNetUserRole();
                    aspnetuserroles.UserId = Aspnetuser.Id;
                    aspnetuserroles.RoleId = "Provider";
                    _context.AspNetUsers.Add(Aspnetuser);
                    _context.SaveChanges();

                    // Physician
                    var Physician = new Physician();
                    Physician.AspNetUserId = Aspnetuser.Id;
                    Physician.FirstName = physiciandata.Firstname;
                    Physician.LastName = physiciandata.Lastname;
                    Physician.Status = physiciandata.Status;
                    Physician.RoleId = physiciandata.Roleid;
                    Physician.Email = physiciandata.Email;
                    Physician.Mobile = physiciandata.Mobile;
                    Physician.MedicalLicense = physiciandata.Medicallicense;
                    Physician.Npinumber = physiciandata.Npinumber;
                    Physician.SyncEmailAddress = physiciandata.Syncemailaddress;
                    Physician.Address1 = physiciandata.Address1;
                    Physician.Address2 = physiciandata.Address2;
                    Physician.City = physiciandata.City;
                    Physician.Zip = physiciandata.Zipcode;
                    Physician.AltPhone = physiciandata.Altphone;
                    Physician.BusinessName = physiciandata.Businessname;
                    Physician.BusinessWebsite = physiciandata.Businesswebsite;
                    Physician.CreatedDate = DateTime.Now;
                    Physician.CreatedBy = AdminId;
                    Physician.RegionId = physiciandata.Regionid;

                    Physician.IsAgreementDoc = new BitArray(1);
                    Physician.IsBackgroundDoc = new BitArray(1);
                    Physician.IsNonDisclosureDoc = new BitArray(1);
                    Physician.IsLicenseDoc = new BitArray(1);
                    Physician.IsTrainingDoc = new BitArray(1);
                    Physician.IsDeleted = new BitArray(1);

                    Physician.IsAgreementDoc[0] = physiciandata.Isagreementdoc;
                    Physician.IsBackgroundDoc[0] = physiciandata.Isbackgrounddoc;
                    //Physician.IsNonDisclosureDoc = physiciandata.Isnondisclosuredoc;
                    Physician.IsLicenseDoc[0] = physiciandata.Islicensedoc;
                    Physician.IsTrainingDoc[0] = physiciandata.Istrainingdoc;
                    Physician.IsDeleted[0] = false;
                    Physician.AdminNotes = physiciandata.Adminnotes;


                    Physician.Photo = physiciandata.PhotoFile != null ? Physician.FirstName + "-" + DateTime.Now.ToString("yyyyMMddhhmmss") + "-Photo." + Path.GetExtension(physiciandata.PhotoFile.FileName).Trim('.') : null;
                    Physician.Signature = physiciandata.SignatureFile != null ? Physician.FirstName + "-" + DateTime.Now.ToString("yyyyMMddhhmmss") + "-Signature.png" : null;



                    _context.Physicians.Add(Physician);
                    _context.SaveChanges();

                    //FileSave.UploadProviderDoc(physiciandata.Agreementdoc, Physician.PhysicianId, "Agreementdoc.pdf");
                    //FileSave.UploadProviderDoc(physiciandata.BackGrounddoc, Physician.PhysicianId, "BackGrounddoc.pdf");
                    //FileSave.UploadProviderDoc(physiciandata.NonDisclosuredoc, Physician.PhysicianId, "NonDisclosuredoc.pdf");
                    //FileSave.UploadProviderDoc(physiciandata.Licensedoc, Physician.PhysicianId, "Agreementdoc.pdf");
                    //FileSave.UploadProviderDoc(physiciandata.Trainingdoc, Physician.PhysicianId, "Trainingdoc.pdf");

                    //FileSave.UploadProviderDoc(physiciandata.SignatureFile, Physician.PhysicianId, Physician.FirstName + "-" + DateTime.Now.ToString("yyyyMMddhhmmss") + "-Signature.png");
                    //FileSave.UploadProviderDoc(physiciandata.PhotoFile, Physician.PhysicianId, Physician.FirstName + "-" + DateTime.Now.ToString("yyyyMMddhhmmss") + "-Photo." + Path.GetExtension(physiciandata.PhotoFile.FileName).Trim('.'));

                    // Physician_region
                    List<int> priceList = physiciandata.Regionsid.Split(',').Select(int.Parse).ToList();
                    foreach (var item in priceList)
                    {
                        PhysicianRegion ar = new PhysicianRegion();
                        ar.RegionId = item;
                        ar.PhysicianId = (int)Physician.PhysicianId;
                        _context.PhysicianRegions.Add(ar);
                        _context.SaveChanges();

                    }
                }
                else
                {

                }
                return true;
            }
            catch (Exception e)
            {

            }
            return false;
        }
    }
}
