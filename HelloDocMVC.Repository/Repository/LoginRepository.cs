﻿using AspNetCoreHero.ToastNotification.Abstractions;
using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HelloDocMVC.Repository.Repository.LoginRepository;

namespace HelloDocMVC.Repository.Repository
{

    public class LoginRepository : ILoginRepository
    {
        #region Constructor
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly HelloDocDbContext _context;
        private readonly INotyfService _notyf;
        private readonly EmailConfiguration _emailConfig;
        public LoginRepository(HelloDocDbContext context, IHttpContextAccessor httpContextAccessor, INotyfService notyf, EmailConfiguration emailConfig)
        {
            this.httpContextAccessor = httpContextAccessor;
            _context = context;
            _notyf = notyf;
            _emailConfig = emailConfig;
        }
        #endregion

        #region Constructor
        public async Task<UserInfo> CheckAccessLogin(AspNetUser aspNetUser)
        {
            var user = await _context.AspNetUsers.FirstOrDefaultAsync(u => u.Email == aspNetUser.Email && u.PasswordHash == aspNetUser.PasswordHash);
            UserInfo admin = new UserInfo();
            if (user != null)
            {
                var hasher = new PasswordHasher<string>();
                //PasswordVerificationResult result = hasher.VerifyHashedPassword(null, user.PasswordHash, aspNetUser.PasswordHash);

                //if (result != PasswordVerificationResult.Success)
                //{
                //    return null;
                //}
                //else
                //{
                    var data = _context.AspNetUserRoles.FirstOrDefault(E => E.UserId == user.Id);
                    var datarole = _context.AspNetRoles.FirstOrDefault(e => e.Id == data.RoleId);
                    admin.Username = user.UserName;
                    admin.FirstName = admin.FirstName ?? string.Empty;
                    admin.LastName = admin.LastName ?? string.Empty;
                    admin.Role = datarole.Name;
                admin.AspNetUserId = user.Id;
                    if (admin.Role == "Admin")
                    {
                        var admindata = _context.Admins.FirstOrDefault(u => u.AspNetUserId == user.Id);
                        admin.UserId = admindata.AdminId;
                    }
                    else if (admin.Role == "Patient")
                    {
                        var admindata = _context.Users.FirstOrDefault(u => u.AspNetUserId == user.Id);
                        admin.UserId = admindata.UserId;
                    }
                    else
                    {
                        var admindata = _context.Physicians.FirstOrDefault(u => u.AspNetUserId == user.Id);
                        admin.UserId = admindata.PhysicianId;
                    }
                    return admin;
                //}
            }
            else
            {
                return null;
            }
        }
        #endregion
        public bool SendResetLink(String Email)
        {
            var agreementUrl = "https://localhost:44338/Login/Resetpass?Email=" + Email;
            _emailConfig.SendMail(Email, "Reset your password", $" Forgot your password? " +
                $"We received a request to reset the password of your account." +
                $"To reset your password,click on the below link <a href='{agreementUrl}'>Reset Password..</a>");
            return true;
        }
    }

}
