using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Entity.Models
{
    public class ViewProvider
    {
        public int? notificationid { get; set; }
        public BitArray? notification { get; set; }
        public int? onCallStatus { get; set; } = 0;
        public int? shiftid { get; set; }
        public string? role { get; set; }
        public int? Physicianid { get; set; }
        public string? Aspnetuserid { get; set; }
        [Required(ErrorMessage = "UserName is required")]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? PassWord { get; set; }
        public string? Regionsid { get; set; }
        [Required(ErrorMessage = "FirstName is required")]
        public string Firstname { get; set; } = null!;
        public string? Lastname { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string Email { get; set; } = null!;
        public string? Mobile { get; set; }
        public string? State { get; set; }
        [Required(ErrorMessage = "ZipCode is required")]
        public string? Zipcode { get; set; }
        public string? Medicallicense { get; set; }
        public string? Photo { get; set; }
        public IFormFile? PhotoFile { get; set; }
        [Required(ErrorMessage = "AdminNotes is Required")]
        public string? Adminnotes { get; set; }
        public bool Isagreementdoc { get; set; }
        public bool Isbackgrounddoc { get; set; }
        public bool Istrainingdoc { get; set; }
        public bool Isnondisclosuredoc { get; set; }
        public bool Islicensedoc { get; set; }
        [Required(ErrorMessage = "Address is Required")]
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        [Required(ErrorMessage = "City is Required")]
        public string? City { get; set; }
        public int? Regionid { get; set; }
        [Required(ErrorMessage = "Phone number is Required")]
        public string? Altphone { get; set; }
        public string? Createdby { get; set; } = null!;
        public DateTime? Createddate { get; set; }
        public string? Modifiedby { get; set; }
        public DateTime? Modifieddate { get; set; }
        public short? Status { get; set; }
        public string Businessname { get; set; } = null!;
        public string Businesswebsite { get; set; } = null!;
        public BitArray? Isdeleted { get; set; }
        public int? Roleid { get; set; }
        public string? Npinumber { get; set; }
        public string? Signature { get; set; }
        public IFormFile? SignatureFile { get; set; }
        public BitArray? Iscredentialdoc { get; set; }
        public BitArray? Istokengenerate { get; set; }
        public string? Syncemailaddress { get; set; }
        public IFormFile? Agreementdoc { get; set; }
        public IFormFile? NonDisclosuredoc { get; set; }
        public IFormFile? Trainingdoc { get; set; }
        public IFormFile? BackGrounddoc { get; set; }
        public IFormFile? Licensedoc { get; set; }
        public List<ViewProvider> ProviderData { get; set; }
        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public bool? IsAscending { get; set; } = true;
        public string? SortedColumn { get; set; } = "PatientName";
        public List<Regions>? Regionids { get; set; }
        public class Regions
        {
            public int? regionid { get; set; }
            public string? regionname { get; set; }
        }
    }
}
