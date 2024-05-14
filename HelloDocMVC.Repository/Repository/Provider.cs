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
        public ViewProvider PhysicianAll(ViewProvider vp)
        {
            List<ViewProvider> data = (from r in _context.Physicians
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
                                                   Email = r.Email,
                                                   Isnondisclosuredoc = (bool)r.IsNonDisclosureDoc
                                               })
                                        .ToList();
            if (vp.IsAscending == true)
            {
                data = vp.SortedColumn switch
                {
                    "PatientName" => data.OrderBy(x => x.Firstname).ToList(),
                    "Role" => data.OrderBy(x => x.role).ToList(),
                    _ => data.OrderBy(x => x.Firstname).ToList()
                };
            }
            else
            {
                data = vp.SortedColumn switch
                {
                    "PatientName" => data.OrderByDescending(x => x.Firstname).ToList(),
                    "Role" => data.OrderByDescending(x => x.role).ToList(),
                    _ => data.OrderByDescending(x => x.Firstname).ToList()
                };
            }
            int totalItemCount = data.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)vp.PageSize);
            List<ViewProvider> list = data.Skip((vp.CurrentPage - 1) * vp.PageSize).Take(vp.PageSize).ToList();

            ViewProvider model = new()
            {
                ProviderData = list,
                CurrentPage = vp.CurrentPage,
                TotalPages = totalPages,
                PageSize = vp.PageSize,
                IsAscending = vp.IsAscending,
                SortedColumn = vp.SortedColumn
            };

            return model;
        }
        public ViewProvider PhysicianByRegion(int? region,ViewProvider vp)
        {
            List<ViewProvider> data = (
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
                                        .ToList();
            if (vp.IsAscending == true)
            {
                data = vp.SortedColumn switch
                {
                    "PatientName" => data.OrderBy(x => x.Firstname).ToList(),
                    "Role" => data.OrderBy(x => x.role).ToList(),
                    _ => data.OrderBy(x => x.Firstname).ToList()
                };
            }
            else
            {
                data = vp.SortedColumn switch
                {
                    "PatientName" => data.OrderByDescending(x => x.Firstname).ToList(),
                    "Role" => data.OrderByDescending(x => x.role).ToList(),
                    _ => data.OrderByDescending(x => x.Firstname).ToList()
                };
            }
            int totalItemCount = data.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)vp.PageSize);
            List<ViewProvider> list = data.Skip((vp.CurrentPage - 1) * vp.PageSize).Take(vp.PageSize).ToList();

            ViewProvider model = new()
            {
                ProviderData = list,
                CurrentPage = vp.CurrentPage,
                TotalPages = totalPages,
                PageSize = vp.PageSize,
                IsAscending = vp.IsAscending,
                SortedColumn = vp.SortedColumn
            };

            return model;
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
                    aspnetuserroles.RoleId = "2";
                    _context.AspNetUserRoles.Add(aspnetuserroles);
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
                    Physician.IsNonDisclosureDoc = false;
                    Physician.IsLicenseDoc = new BitArray(1);
                    Physician.IsTrainingDoc = new BitArray(1);
                    Physician.IsDeleted = new BitArray(1);

                    Physician.IsAgreementDoc[0] = physiciandata.Isagreementdoc;
                    Physician.IsBackgroundDoc[0] = physiciandata.Isbackgrounddoc;
                    Physician.IsNonDisclosureDoc = physiciandata.Isnondisclosuredoc;
                    Physician.IsLicenseDoc[0] = physiciandata.Islicensedoc;
                    Physician.IsTrainingDoc[0] = physiciandata.Istrainingdoc;
                    Physician.IsDeleted[0] = false;
                    Physician.AdminNotes = physiciandata.Adminnotes;


                    Physician.Photo = physiciandata.PhotoFile != null ? Physician.FirstName + "-" + DateTime.Now.ToString("yyyyMMddhhmmss") + "-Photo." + Path.GetExtension(physiciandata.PhotoFile.FileName).Trim('.') : null;
                    Physician.Signature = physiciandata.SignatureFile != null ? Physician.FirstName + "-" + DateTime.Now.ToString("yyyyMMddhhmmss") + "-Signature.png" : null;



                    _context.Physicians.Add(Physician);
                    _context.SaveChanges();

                    FileSave.UploadProviderDoc(physiciandata.Agreementdoc, Physician.PhysicianId, "Agreementdoc.pdf");
                    FileSave.UploadProviderDoc(physiciandata.BackGrounddoc, Physician.PhysicianId, "BackGrounddoc.pdf");
                    FileSave.UploadProviderDoc(physiciandata.NonDisclosuredoc, Physician.PhysicianId, "NonDisclosuredoc.pdf");
                    FileSave.UploadProviderDoc(physiciandata.Licensedoc, Physician.PhysicianId, "Agreementdoc.pdf");
                    FileSave.UploadProviderDoc(physiciandata.Trainingdoc, Physician.PhysicianId, "Trainingdoc.pdf");

                    FileSave.UploadProviderDoc(physiciandata.SignatureFile, Physician.PhysicianId, Physician.FirstName + "-" + DateTime.Now.ToString("yyyyMMddhhmmss") + "-Signature.png");
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
        public async Task<ViewProvider> GetPhysicianById(int id)
        {


            ViewProvider? pl = await (from r in _context.Physicians
                                        join Aspnetuser in _context.AspNetUsers
                                        on r.AspNetUserId equals Aspnetuser.Id into aspGroup
                                        from asp in aspGroup.DefaultIfEmpty()
                                        join Notifications in _context.PhysicianNotifications
                                         on r.PhysicianId equals Notifications.PhysicianId into PhyNGroup
                                        from nof in PhyNGroup.DefaultIfEmpty()
                                        join role in _context.Roles
                                        on r.RoleId equals role.RoleId into roleGroup
                                        from roles in roleGroup.DefaultIfEmpty()
                                        where r.PhysicianId == id
                                        select new ViewProvider
                                        {
                                            UserName = asp.UserName,
                                            Roleid = r.RoleId,
                                            Status = r.Status,
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
                                            Email = r.Email,
                                            Photo = r.Photo,
                                            Signature = r.Signature,
                                            Isagreementdoc = r.IsAgreementDoc[0],
                                            Isnondisclosuredoc = r.IsNonDisclosureDoc == false ? false : true,
                                            Isbackgrounddoc = r.IsBackgroundDoc[0],
                                            Islicensedoc = r.IsLicenseDoc[0],
                                            Istrainingdoc = r.IsTrainingDoc[0],
                                            Medicallicense = r.MedicalLicense,
                                            Npinumber = r.Npinumber,
                                            Createdby = r.CreatedBy,
                                            Syncemailaddress = r.SyncEmailAddress,
                                            Zipcode = r.Zip,
                                            Regionid = r.RegionId

                                        })
                                   .FirstOrDefaultAsync();

            List<ViewProvider.Regions> regions = new List<ViewProvider.Regions>();

            regions = _context.PhysicianRegions
                  .Where(r => r.PhysicianId == pl.Physicianid)
                  .Select(req => new ViewProvider.Regions()
                  {
                      regionid = req.RegionId
                  })
                  .ToList();

            pl.Regionids = regions;
            return pl;
        }
        public async Task<bool> EditAccountInfo(ViewProvider vm)
        {
            try
            {
                if (vm == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Physicians
                        .Where(W => W.PhysicianId == vm.Physicianid)
                        .FirstOrDefaultAsync();
                    AspNetUser User = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Id == DataForChange.AspNetUserId);
                    if (DataForChange != null && User != null)
                    {
                        User.UserName = vm.UserName;
                        DataForChange.Status = vm.Status;
                        DataForChange.RoleId = vm.Roleid;
                        _context.Physicians.Update(DataForChange);
                        _context.AspNetUsers.Update(User);
                        _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> ChangePasswordAsync(string password, int Physicianid)
        {
            var hasher = new PasswordHasher<string>();
            var req = await _context.Physicians
                .Where(W => W.PhysicianId == Physicianid)
                    .FirstOrDefaultAsync();
            if (req != null)
            {
                var User = await _context.AspNetUsers.Where(m => m.Id == req.AspNetUserId).FirstOrDefaultAsync();
                if (User != null)
                {
                    User.PasswordHash = hasher.HashPassword(null, password);
                    _context.AspNetUsers.Update(User);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            return false;
        }
        public async Task<bool> EditPhysicianInfo(ViewProvider vm)
        {
            try
            {
                if (vm == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Physicians
                        .Where(W => W.PhysicianId == vm.Physicianid)
                        .FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        DataForChange.FirstName = vm.Firstname;
                        DataForChange.LastName = vm.Lastname;
                        DataForChange.Email = vm.Email;
                        DataForChange.Mobile = vm.Mobile;
                        DataForChange.MedicalLicense = vm.Medicallicense;
                        DataForChange.Npinumber = vm.Npinumber;
                        DataForChange.SyncEmailAddress = vm.Syncemailaddress;
                        _context.Physicians.Update(DataForChange);
                        _context.SaveChanges();
                        List<int> regions = await _context.PhysicianRegions.Where(r => r.PhysicianId == vm.Physicianid).Select(req => req.RegionId).ToListAsync();
                        List<int> priceList = vm.Regionsid.Split(',').Select(int.Parse).ToList();
                        foreach (var item in priceList)
                        {
                            if (regions.Contains(item))
                            {
                                regions.Remove(item);
                            }
                            else
                            {
                                PhysicianRegion pr = new()
                                {
                                    RegionId = item,
                                    PhysicianId = (int)vm.Physicianid
                                };
                                _context.PhysicianRegions.Update(pr);
                                await _context.SaveChangesAsync();
                                regions.Remove(item);
                            }
                        }
                        if (regions.Count > 0)
                        {
                            foreach (var item in regions)
                            {
                                PhysicianRegion pr = await _context.PhysicianRegions.Where(r => r.PhysicianId == vm.Physicianid && r.RegionId == item).FirstAsync();
                                _context.PhysicianRegions.Remove(pr);
                                await _context.SaveChangesAsync();
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> EditMailBillingInfo(ViewProvider vm, string AdminId)
        {
            try
            {
                if (vm == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Physicians
                        .Where(W => W.PhysicianId == vm.Physicianid)
                        .FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        DataForChange.Address1 = vm.Address1;
                        DataForChange.Address2 = vm.Address2;
                        DataForChange.City = vm.City;
                        DataForChange.RegionId = vm.Regionid;
                        DataForChange.Zip = vm.Zipcode;
                        DataForChange.AltPhone = vm.Altphone;
                        DataForChange.ModifiedBy = AdminId;
                        DataForChange.ModifiedDate = DateTime.Now;
                        _context.Physicians.Update(DataForChange);
                        _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> EditProviderProfile(ViewProvider vm, string AdminId)
        {
            try
            {
                if (vm == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Physicians
                        .Where(W => W.PhysicianId == vm.Physicianid)
                        .FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        if (vm.PhotoFile != null)
                        {
                            DataForChange.Photo = vm.PhotoFile != null ? vm.Firstname + "-" + DateTime.Now.ToString("yyyyMMddhhmm") + "-Photo." + Path.GetExtension(vm.PhotoFile.FileName).Trim('.') : null;
                            FileSave.UploadProviderDoc(vm.PhotoFile, (int)vm.Physicianid, vm.Firstname + "-" + DateTime.Now.ToString("yyyyMMddhhmm") + "-Photo." + Path.GetExtension(vm.PhotoFile.FileName).Trim('.'));

                        }
                        if (vm.SignatureFile != null)
                        {
                            DataForChange.Signature = vm.SignatureFile != null ? vm.Firstname + "-" + DateTime.Now.ToString("yyyyMMddhhmm") + "-Signature.png" : null;
                            FileSave.UploadProviderDoc(vm.SignatureFile, (int)vm.Physicianid, vm.Firstname + "-" + DateTime.Now.ToString("yyyyMMddhhmm") + "-Signature.png");
                        }
                        DataForChange.BusinessName = vm.Businessname;
                        DataForChange.BusinessWebsite = vm.Businesswebsite;
                        DataForChange.ModifiedBy = AdminId;
                        DataForChange.AdminNotes = vm.Adminnotes;
                        DataForChange.ModifiedDate = DateTime.Now;
                        _context.Physicians.Update(DataForChange);
                        _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> EditProviderOnbording(ViewProvider vm, string AdminId)
        {
            try
            {
                if (vm == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Physicians
                        .Where(W => W.PhysicianId == vm.Physicianid)
                        .FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        FileSave.UploadProviderDoc(vm.Agreementdoc, (int)vm.Physicianid, "Agreementdoc.pdf");
                        FileSave.UploadProviderDoc(vm.BackGrounddoc, (int)vm.Physicianid, "BackGrounddoc.pdf");
                        FileSave.UploadProviderDoc(vm.NonDisclosuredoc, (int)vm.Physicianid, "NonDisclosuredoc.pdf");
                        FileSave.UploadProviderDoc(vm.Licensedoc, (int)vm.Physicianid, "Agreementdoc.pdf");
                        FileSave.UploadProviderDoc(vm.Trainingdoc, (int)vm.Physicianid, "Trainingdoc.pdf");

                        DataForChange.IsAgreementDoc = new BitArray(1);
                        DataForChange.IsBackgroundDoc = new BitArray(1);
                        DataForChange.IsNonDisclosureDoc = false;
                        DataForChange.IsLicenseDoc = new BitArray(1);
                        DataForChange.IsTrainingDoc = new BitArray(1);

                        DataForChange.IsAgreementDoc[0] = vm.Isagreementdoc;
                        DataForChange.IsBackgroundDoc[0] = vm.Isbackgrounddoc;
                        DataForChange.IsNonDisclosureDoc = vm.Isnondisclosuredoc;
                        DataForChange.IsLicenseDoc[0] = vm.Islicensedoc;
                        DataForChange.IsTrainingDoc[0] = vm.Istrainingdoc;
                        DataForChange.ModifiedBy = AdminId;
                        DataForChange.ModifiedDate = DateTime.Now;

                        _context.Physicians.Update(DataForChange);
                        _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> DeletePhysician(int PhysicianID, string AdminID)
        {
            try
            {
                BitArray bt = new BitArray(1);
                bt.Set(0, true);
                if (PhysicianID == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Physicians
                        .Where(W => W.PhysicianId == PhysicianID)
                        .FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        DataForChange.IsDeleted = bt;
                        DataForChange.IsDeleted[0] = true;
                        DataForChange.ModifiedDate = DateTime.Now;
                        DataForChange.ModifiedBy = AdminID;
                        _context.Physicians.Update(DataForChange);
                        _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<PhysicianLocation> FindPhysicianLocation()
        {
            List<PhysicianLocation> pl = _context.PhysicianLocations
                                    .OrderByDescending(x => x.PhysicianName)
                        .Select(r => new PhysicianLocation
                        {
                            LocationId = r.LocationId,
                            Longitude = r.Longitude,
                            Latitude = r.Latitude,
                            PhysicianName = r.PhysicianName

                        }).ToList();
            return pl;

        }
        public bool SendMessage(string? Message)
        {
            string contact = "+917801870065";
            bool sms = _emailConfig.SendSMS(contact, Message).Result;
            return sms;
        }
        public Payrate GetPayrate(int id)
        {
            Payrate? p = (from r in _context.PhysicianPayrates
                         where r.PhysicianId == id
                         select new Payrate
                         {
                             PhysicianId = id,
                             NightShiftWeekend = r.NightShiftWeekend,
                             Shift = r.Shift,
                             HouseCallNightsWeekend = r.HouseCallNightsWeekend,
                             PhoneConsults = r.PhoneConsults,
                             PhoneConsultsNightsWeekend = r.PhoneConsultsNightsWeekend,
                             BatchTesting = r.BatchTesting,
                             HouseCalls = r.HouseCalls,
                         }).FirstOrDefault();
            if (p == null)
            {
                Payrate pr = new();
                pr.PhysicianId = id;
                return pr;
            }
            return p;
        }
        public bool PayratePost(Payrate pr, string id)
        {
            try
            {
                if (pr == null)
                {
                    return false;
                }
                else
                {
                    var p = _context.PhysicianPayrates.Where(W => W.PhysicianId == pr.PhysicianId).FirstOrDefault();
                    if (p == null)
                    {
                        p = new PhysicianPayrate { PhysicianId = pr.PhysicianId, CreatedBy = id, CreatedDate = DateTime.Now };
                        _context.PhysicianPayrates.Add(p);
                    }
                    p.NightShiftWeekend = pr.NightShiftWeekend == null ? p.NightShiftWeekend : pr.NightShiftWeekend;
                    p.Shift = pr.Shift == null ? p.Shift : pr.Shift;
                    p.HouseCallNightsWeekend = pr.HouseCallNightsWeekend == null ? p.HouseCallNightsWeekend : pr.HouseCallNightsWeekend;
                    p.PhoneConsults = pr.PhoneConsults == null ? p.PhoneConsults : pr.PhoneConsults;
                    p.PhoneConsultsNightsWeekend = pr.PhoneConsultsNightsWeekend == null ? p.PhoneConsultsNightsWeekend : pr.PhoneConsultsNightsWeekend;
                    p.BatchTesting = pr.BatchTesting == null ? p.BatchTesting : pr.BatchTesting;
                    p.HouseCalls = pr.HouseCalls == null ? p.HouseCalls : pr.HouseCalls;
                    if (p != null)
                    {
                        p.ModifiedBy = id;
                        p.ModifiedDate = DateTime.Now;
                    }
                    _context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
