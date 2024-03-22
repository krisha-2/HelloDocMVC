using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static HelloDocMVC.Entity.Models.ViewAdminProfileData;

namespace HelloDocMVC.Repository.Repository
{
    public class AdminProfile : IAdminProfile
    {
        private readonly HelloDocDbContext _context;
        public AdminProfile(HelloDocDbContext context)
        {
            _context = context;
        }
        #region GetProfile
        public async Task<ViewAdminProfileData> GetProfileDetails(int UserId)
        {
            ViewAdminProfileData? v = await (from r in _context.Admins
                                         join AspNetUser in _context.AspNetUsers
                                         on r.AspNetUserId equals AspNetUser.Id into aspGroup
                                         from asp in aspGroup.DefaultIfEmpty()
                                         where r.AdminId == UserId
                                         select new ViewAdminProfileData
                                         {
                                             Roleid = r.RoleId,
                                             AdminId = r.AdminId,
                                             UserName = asp.UserName,
                                             Address1 = r.Address1,
                                             Address2 = r.Address2,
                                             AltMobile = r.AltPhone,
                                             City = r.City,
                                             Aspnetuserid = r.AspNetUserId,
                                             Createdby = r.CreatedBy,
                                             Email = r.Email,
                                             Createddate = r.CreatedDate,
                                             Mobile = r.Mobile,
                                             Modifiedby = r.ModifiedBy,
                                             Modifieddate = r.ModifiedDate,
                                             Regionid = r.RegionId,
                                             LastName = r.LastName,
                                             FirstName = r.FirstName,
                                             Status = r.Status,
                                             Zip = r.Zip
                                         }).FirstOrDefaultAsync();
            List<Regions> regions = new List<Regions>();
            regions = await _context.AdminRegions
                  .Where(r => r.AdminId == UserId)
                  .Select(req => new Regions()
                  {
                      regionid = req.RegionId
                  })
                  .ToListAsync();
            v.Regionids = regions;
            return v;
        }
        #endregion
        #region EditPassword
        public async Task<bool> EditPassword(string password, int adminId)
        {
            var hasher = new PasswordHasher<string>();
            var req = await _context.Admins.Where(W => W.AdminId == adminId).FirstOrDefaultAsync();
            AspNetUser? U = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Id == req.AspNetUserId);

            if (U != null)
            {
                U.PasswordHash = hasher.HashPassword(null, password);
                _context.AspNetUsers.Update(U);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion
        #region EditAdministratorInfo
        public bool EditAdministratorInfo(ViewAdminProfileData _viewAdminProfile)
        {
            try
            {
                if (_viewAdminProfile == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange =  _context.Admins.Where(W => W.AdminId == _viewAdminProfile.AdminId).FirstOrDefault();
                    if (DataForChange != null)
                    {
                        DataForChange.Email = _viewAdminProfile.Email;
                        DataForChange.FirstName = _viewAdminProfile.FirstName;
                        DataForChange.LastName = _viewAdminProfile.LastName;
                        DataForChange.Mobile = _viewAdminProfile.Mobile;
                        _context.Admins.Update(DataForChange);
                        _context.SaveChanges();
                        List<int> regions =  _context.AdminRegions.Where(r => r.AdminId == _viewAdminProfile.AdminId).Select(req => req.RegionId).ToList();
                        List<int> priceList = _viewAdminProfile.Regionsid.Split(',').Select(int.Parse).ToList();
                        foreach (var item in priceList)
                        {
                            if (regions.Contains(item))
                            {
                                regions.Remove(item);
                            }
                            else
                            {
                                AdminRegion ar = new() { RegionId = item, AdminId = (int)_viewAdminProfile.AdminId };
                                _context.AdminRegions.Update(ar);
                                 _context.SaveChanges();
                            }
                        }
                        if (regions.Count > 0)
                        {
                            foreach (var item in regions)
                            {
                                AdminRegion ar =  _context.AdminRegions.Where(r => r.AdminId == _viewAdminProfile.AdminId && r.RegionId == item).First();
                                _context.AdminRegions.Remove(ar);
                                 _context.SaveChanges();
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
        #endregion
        #region BillingInfoEdit
        public async Task<bool> BillingInfoEdit(ViewAdminProfileData _viewAdminProfile)
        {
            try
            {
                if (_viewAdminProfile == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Admins.Where(W => W.AdminId == _viewAdminProfile.AdminId).FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        DataForChange.Address1 = _viewAdminProfile.Address1;
                        DataForChange.Address2 = _viewAdminProfile.Address2;
                        DataForChange.City = _viewAdminProfile.City;
                        DataForChange.Mobile = _viewAdminProfile.Mobile;
                        _context.Admins.Update(DataForChange);
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
        #endregion
    }
}
