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
        #region PhysicianAll
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
        #endregion
        #region Change_Notification_Physician
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
        #endregion
    }
}
