using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models;
using Microsoft.AspNetCore.Mvc;
using HelloDocMVC.Repository.Repository.Interface;
using HelloDocMVC.Repository.Repository;
using HelloDocMVC.Models;

namespace HelloDocMVC.Controllers
{
    public class AccessController : Controller
    {
        #region Constructor
        private readonly IAccessRole _IAccessRole;
        private readonly INotyfService _notyf;
        private readonly IComboBox _comboBox;
        private readonly ILogger<AdminController> _logger;
        private readonly EmailConfiguration _emailConfig;

        public AccessController(ILogger<AdminController> logger, IAccessRole IRoleAccessRepository, INotyfService notyf, IComboBox comboBox, EmailConfiguration emailConfiguration)
        {
            _IAccessRole = IRoleAccessRepository;
            _notyf = notyf;
            _logger = logger;
            _comboBox = comboBox;
            _emailConfig = emailConfiguration;
        }
        #endregion
        #region Index
        public async Task<IActionResult> Index()
        {
            List<Role> v = await _IAccessRole.GetRoleAccessDetails();
            return View("../Access/Index", v);
        }
		#endregion
		#region User_Access
		public async Task<IActionResult> UserAccess(int? User)
		{
			List<ViewUser> data = await _IAccessRole.GetAllUserDetails(User);
			if (User != null)
			{
				return Json(data);
			}
			return View("../Access/UserAccess", data);
		}
		#endregion

		#region Create_Role_Access-ADDEdit
		public async Task<IActionResult> CreateRoleAccess(int? id)
		{
			if (id != null)
			{
				ViewData["RolesAddEdit"] = "Edit";
				ViewRole v = await _IAccessRole.GetRoleByMenus((int)id);
				return View("../Access/CreateRole", v);
			}
			ViewData["RolesAddEdit"] = "Create";

			return View("../Access/CreateRole");
		}
		#endregion

		#region GetMenusByAccount
		public async Task<IActionResult> GetMenusByAccount(short Accounttype, int roleid)
		{
			if (Accounttype == 0)
			{
				List<Menu> data = await _IAccessRole.GetMenusByAccount(1);
				return Json(data);
			}
			List<Menu> v = await _IAccessRole.GetMenusByAccount(Accounttype);

			if (roleid != null)
			{
				List<ViewRole.Menu> vm = new List<ViewRole.Menu>();
				List<int> rm = await _IAccessRole.CheckMenuByRole(roleid);
				foreach (var item in v)
				{
					ViewRole.Menu menu = new ViewRole.Menu();
					menu.Name = item.Name;
					menu.Menuid = item.MenuId;

					if (rm.Contains(item.MenuId))
					{
						menu.checekd = "checked";
						vm.Add(menu);
					}
					else
					{
						vm.Add(menu);
					}
				}
				return Json(vm);
			}

			return Json(v);
		}
		#endregion

		#region PostRoleMenu
		public async Task<IActionResult> PostRoleMenu(ViewRole role, string Menusid)
		{
			if (role.Roleid == 0)
			{
				if (await _IAccessRole.PostRoleMenu(role, Menusid, CV.ID()))
				{
					_notyf.Success("Role Add Successfully...");
				}
				else
				{
					_notyf.Error("Role not Add...");
				}
			}
			else
			{
				if (await _IAccessRole.PutRoleMenu(role, Menusid, CV.ID()))
				{
					_notyf.Success("Role Modified Successfully...");
				}
				else
				{
					_notyf.Error("Role not Modified...");
				}
			}
			return RedirectToAction("Index");
		}
		#endregion

		#region DeletePhysician
		public async Task<IActionResult> DeleteRole(int roleid)
		{
			bool data = await _IAccessRole.DeleteRoles(roleid, CV.ID());
			if (data)
			{
				_notyf.Success("Role deleted successfully...");
				return RedirectToAction("Index");
			}
			else
			{
				_notyf.Success("Role not deleted successfully...");
				return RedirectToAction("Index");
			}
		}
		#endregion
	}
}