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
        private readonly IAccessRole _IAccessRole;
        private readonly INotyfService _notyf;
        private readonly IComboBox _comboBox;
        private readonly ILogger<AdminController> _logger;
        private readonly EmailConfiguration _emailConfig;
        private readonly IAdminProfile _IAdminProfile;
        private readonly IProvider _IProvider;
        public AccessController(ILogger<AdminController> logger, IAccessRole IRoleAccessRepository, INotyfService notyf, IComboBox comboBox, EmailConfiguration emailConfiguration, IAdminProfile IMyProfileRepository, IProvider IProvider)
        {
            _IAccessRole = IRoleAccessRepository;
            _notyf = notyf;
            _logger = logger;
            _comboBox = comboBox;
            _emailConfig = emailConfiguration;
            _IAdminProfile = IMyProfileRepository;
            _IProvider = IProvider;
        }
        public async Task<IActionResult> Index()
        {
            List<Role> v = await _IAccessRole.GetRoleAccessDetails();
            return View("../Access/Index", v);
        }
        public async Task<IActionResult> UserRole(int? role)
        {
            List<ViewUser> data = await _IAccessRole.GetAllUserDetails(role);
            if (role != null)
            {
                data = await _IAccessRole.GetAllUserDetails(role);
            }
            return View("../Access/UserRole", data);
		}
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
		public async Task<IActionResult> PostRoleMenu(ViewRole role, string Menusid)
		{
            if (Menusid != null)
            {
                if (role.Roleid == 0)
                {
                    if (await _IAccessRole.PostRoleMenu(role, Menusid, CV.ID()))
                    {
                        _notyf.Success("Role Added Successfully...");
                    }
                    else
                    {
                        _notyf.Error("Role not Added...");
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
            }
            else
            {
                TempData["Errormessage"] = "Please Select one of the role!!";
                return View("../Access/CreateRole");
            }
			
			return RedirectToAction("Index");
		}
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
        public async Task<IActionResult> PhysicianAddEdit(int? id)
        {
            ViewBag.RegionComboBox = _comboBox.RegionComboBox();
            ViewBag.PhysicianRoleComboBox = await _comboBox.PhysicianRoleComboBox();
            if (id == null)
            {
                ViewData["PhysicianAccount"] = "Add";
            }
            else
            {
                ViewData["PhysicianAccount"] = "Edit";
                ViewProvider v = await _IProvider.GetPhysicianById((int)id);
                return View("../Provider/Edit", v);
            }
            return View("../Provider/Edit");
        }
        public async Task<IActionResult> AdminAddEdit(int? id)
        {


            //TempData["Status"] = TempData["Status"];
            ViewBag.RegionComboBox = _comboBox.RegionComboBox();
            ViewBag.UserRolecombobox = await _comboBox.UserRoleComboBox();
            if (id == null)
            {
                ViewData["AdminAccount"] = "Add Admin";
            }
            return View("../Access/AdminAddEdit");
        }
        public async Task<IActionResult> AdminEdit(int? id)
        {
            ViewAdminProfileData p = await _IAdminProfile.GetProfileDetails((id != null ? (int)id : Convert.ToInt32(CV.UserID())));
            ViewBag.RegionComboBox = _comboBox.RegionComboBox();
            ViewBag.UserRolecombobox = await _comboBox.UserRoleComboBox();
            return View("../AdminProfile/Index", p);
        }
        [HttpPost]
        public async Task<IActionResult> AdminAdd(ViewAdminProfileData vm)
        {
            //TempData["Status"] = TempData["Status"];
            ViewBag.RegionComboBox = _comboBox.RegionComboBox();
            ViewBag.UserRolecombobox = await _comboBox.AdminRoleComboBox();
            // bool b = physicians.Isagreementdoc[0];

            /*if (ModelState.IsValid)
            {*/
            if (await _IAdminProfile.AdminPost(vm, CV.ID()))
            {
                _notyf.Success("Admin Added Successfully..!");
            }
            else
            {
                _notyf.Error("Admin not Added Successfully..!");
                return View("../Access/AdminAddEdit", vm);
            }
            /*else
            {
                return View("../Admin/Access/AdminAddEdit", vm);
            }*/
            return RedirectToAction("UserRole");
        }
        public async Task<IActionResult> SaveAdminInfo(ViewAdminProfileData vm)
        {
            bool data = await _IAdminProfile.SaveAdminInfo(vm);
            if (data)
            {
                _notyf.Success("Admin Information Changed successfully...");
            }
            else
            {
                _notyf.Error("Admin Information not Changed successfully...");
            }
            return RedirectToAction("AdminEdit", new { id = vm.AdminId });
        }
        public async Task<IActionResult> EditAdministratorInfo(ViewAdminProfileData vm)
        {
            //bool data = await _IAdminProfile.EditAdministratorInfo(vm);
            //if (data)
            //{
            //    _notyf.Success("Administration Information Changed successfully...");
            //}
            //else
            //{
            //    _notyf.Error("Administration Information not Changed successfully...");
            //}
            return RedirectToAction("AdminEdit", new { id = vm.AdminId });
        }
        public async Task<IActionResult> BillingInfoEdit(ViewAdminProfileData vm)
        {
            bool data = await _IAdminProfile.BillingInfoEdit(vm);
            if (data)
            {
                _notyf.Success("Billing Information Changed successfully...");
            }
            else
            {
                _notyf.Error("Billing Information not Changed successfully...");
            }
            return RedirectToAction("AdminEdit", new { id = vm.AdminId });
        }
        public async Task<IActionResult> EditPassword(string password, int AdminId)
        {
            bool data = await _IAdminProfile.EditPassword(password, AdminId);
            if (data)
            {
                _notyf.Success("Password changed Successfully...");
            }
            else
            {
                _notyf.Error("Password not Changed Successfully...");
            }
            return RedirectToAction("AdminEdit", new { id = AdminId });
        }
    }
}