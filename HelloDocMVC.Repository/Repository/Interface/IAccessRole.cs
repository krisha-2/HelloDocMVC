using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Repository.Repository.Interface
{
	public interface IAccessRole
	{
		public Task<List<Role>> GetRoleAccessDetails();
        public Task<List<ViewUser>> GetAllUserDetails(int? User);
        public Task<ViewRole> GetRoleByMenus(int roleid);
        public Task<List<HelloDocMVC.Entity.DataModels.Menu>> GetMenusByAccount(short Accounttype);
        public Task<List<int>> CheckMenuByRole(int roleid);
        public Task<bool> PostRoleMenu(ViewRole role, string Menusid, string ID);
        public Task<bool> PutRoleMenu(ViewRole role, string Menusid, string ID);
        public Task<bool> DeleteRoles(int roleid, string AdminID);
    }
}
