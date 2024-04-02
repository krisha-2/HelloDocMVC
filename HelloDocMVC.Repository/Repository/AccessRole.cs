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
	public class AccessRole : IAccessRole
	{
		private readonly HelloDocDbContext _context;
		private readonly EmailConfiguration _emailConfig;
		public AccessRole(HelloDocDbContext context, EmailConfiguration emailConfig)
		{
			_context = context;
			_emailConfig = emailConfig;
		}
		#region GetRoleAccessDetails
		public async Task<List<Role>> GetRoleAccessDetails()
		{
            List<Role> v = await _context.Roles.Where(r => r.IsDeleted == new BitArray(1)).ToListAsync();
            return v;
		}
        #endregion
        #region GetAllUserDetails
        public async Task<List<ViewUser>> GetAllUserDetails(int? User)
        {
            IQueryable<ViewUser> query =
                from user in _context.AspNetUsers
                join admin in _context.Admins on user.Id equals admin.AspNetUserId into adminGroup
                from admin in adminGroup.DefaultIfEmpty()
                join physician in _context.Physicians on user.Id equals physician.AspNetUserId into physicianGroup
                from physician in physicianGroup.DefaultIfEmpty()
                join req in _context.Requests on physician.PhysicianId equals req.PhysicianId into totalrequestgroup
                from req in totalrequestgroup.DefaultIfEmpty()
                where (admin != null || physician != null) &&
                      (admin.IsDeleted == new BitArray(1) || physician.IsDeleted == new BitArray(1))
                select new ViewUser
                {
                    UserName = user.UserName,
                    FirstName = admin != null ? admin.FirstName : (physician != null ? physician.FirstName : null),
                    isAdmin = admin != null,
                    UserID = admin != null ? admin.AdminId : (physician != null ? physician.PhysicianId : null),
                    accounttype = admin != null ? 2 : (physician != null ? 3 : null),
                    status = admin != null ? admin.Status : (physician != null ? physician.Status : null),
                    Mobile = admin != null ? admin.Mobile : (physician != null ? physician.Mobile : null),
                };

            if (User.HasValue)
            {
                switch (User.Value)
                {
                    case 2: // Admin data
                        query = query.Where(u => u.isAdmin);
                        break;
                    case 3: // Provider data
                        query = query.Where(u => !u.isAdmin);
                        break;
                }
            }
            return await query.ToListAsync();
        }
        #endregion

        #region GetRoleByMenus
        public async Task<ViewRole> GetRoleByMenus(int roleid)
        {
            var r = await _context.Roles
                        .Where(r => r.RoleId == roleid)
                        .Select(r => new ViewRole
                        {
                            Accounttype = r.AccountType,
                            Createdby = r.CreatedBy,
                            Roleid = r.RoleId,
                            Name = r.Name,
                            Isdeleted = r.IsDeleted
                        })
                        .FirstOrDefaultAsync();
            return r;
        }
        #endregion
        #region GetMenusByAccount
        public async Task<List<HelloDocMVC.Entity.DataModels.Menu>> GetMenusByAccount(short Accounttype)
        {
            return await _context.Menus.Where(r => r.AccountType == Accounttype).ToListAsync();
        }
        #endregion
        #region CheckMenuByRole
        public async Task<List<int>> CheckMenuByRole(int roleid)
        {
            return await _context.RoleMenus
                        .Where(r => r.RoleId == roleid)
                        .Select(r => r.MenuId)
                        .ToListAsync();
        }
        #endregion
        #region PostRoleMenu
        public async Task<bool> PostRoleMenu(ViewRole role, string Menusid, string ID)
        {
            try
            {
                Role check = await _context.Roles.Where(r => r.Name == role.Name).FirstOrDefaultAsync();
                if (check == null && role != null && Menusid != null)
                {
                    Role r = new Role();
                    r.Name = role.Name;
                    r.AccountType = role.Accounttype;
                    r.CreatedBy = ID;
                    r.CreatedDate = DateTime.Now;
                    r.IsDeleted = new System.Collections.BitArray(1);
                    r.IsDeleted[0] = false;
                    _context.Roles.Add(r);
                    _context.SaveChanges();
                    List<int> priceList = Menusid.Split(',').Select(int.Parse).ToList();
                    foreach (var item in priceList)
                    {
                        RoleMenu ar = new RoleMenu();
                        ar.RoleId = r.RoleId;
                        ar.MenuId = item;
                        _context.RoleMenus.Add(ar);
                    }
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        #endregion

        #region PutRoleMenu
        public async Task<bool> PutRoleMenu(ViewRole role, string Menusid, string ID)
        {
            try
            {
                Role check = await _context.Roles.Where(r => r.RoleId == role.Roleid).FirstOrDefaultAsync();
                if (check != null && role != null && Menusid != null)
                {
                    check.Name = role.Name;
                    check.AccountType = role.Accounttype;
                    check.ModifiedBy = ID;
                    check.ModifiedDate = DateTime.Now;
                    _context.Roles.Update(check);
                    _context.SaveChanges();
                    List<int> regions = await CheckMenuByRole(check.RoleId);
                    List<int> priceList = Menusid.Split(',').Select(int.Parse).ToList();
                    foreach (var item in priceList)
                    {
                        if (regions.Contains(item))
                        {
                            regions.Remove(item);
                        }
                        else
                        {
                            RoleMenu ar = new RoleMenu();
                            ar.MenuId = item;
                            ar.RoleId = check.RoleId;
                            _context.RoleMenus.Update(ar);
                            await _context.SaveChangesAsync();
                            regions.Remove(item);
                        }
                    }
                    if (regions.Count > 0)
                    {
                        foreach (var item in regions)
                        {
                            RoleMenu ar = await _context.RoleMenus.Where(r => r.RoleId == check.RoleId && r.MenuId == item).FirstAsync();
                            _context.RoleMenus.Remove(ar);
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
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region DeletePhysician
        public async Task<bool> DeleteRoles(int roleid, string AdminID)
        {
            try
            {
                BitArray bt = new BitArray(1);
                bt.Set(0, true);
                if (roleid == null)
                {
                    return false;
                }
                else
                {
                    var DataForChange = await _context.Roles
                        .Where(W => W.RoleId == roleid)
                        .FirstOrDefaultAsync();
                    if (DataForChange != null)
                    {
                        DataForChange.IsDeleted = bt;
                        DataForChange.IsDeleted[0] = true;
                        DataForChange.ModifiedDate = DateTime.Now;
                        DataForChange.ModifiedBy = AdminID;
                        _context.Roles.Update(DataForChange);
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
