using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Entity.Models
{
    public class ViewAdminProfileData
    {
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        public int? Status { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "FirstName is required")]
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Mobile { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? State { get; set; }
        [Required(ErrorMessage = "City is required")]
        public string? City { get; set; }
        [Required(ErrorMessage = "ZipCode is required")]
        public string? Zip { get; set; }
        public int? AdminId { get; set; }
        public string? Aspnetuserid { get; set; }
        public int? Roleid { get; set; }
        public string? AltMobile { get; set; }
        public string? Street { get; set; }
        public int? Regionid { get; set; }
        public string? Regionsid { get; set; }
        public List<Regions>? Regionids { get; set; }
        public string? Createdby { get; set; } = null!;
        public DateTime? Createddate { get; set; }
        public string? Modifiedby { get; set; }
        public DateTime? Modifieddate { get; set; }
        public string? Ip { get; set; }
        public class Regions
        {
            public int? regionid { get; set; }
            public string? regionname { get; set; }
        }
    }
}
