using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace HelloDocMVC.Entity.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string PasswordHash { get; set; }
        
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string RFirstName { get; set; }
        public string? RLastName { get; set; }
        public string RcFirstName { get; set; }
        public string? RcLastName { get; set; }
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "PhoneNumber is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile Number must be of 10 digits!")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Mobile Number must contain digits!")]
        public string Mobile { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string Email { get; set; }
        public string? RelationWithPatient { get; set; }
        public string? RelationName { get; set; }
        public string? PatientDetails { get; set; }
        public string? Symptoms { get; set; }
        public DateTime DOB { get; set; }
        public string? Contact { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [Required(ErrorMessage = "ZipCode is Required")]
        //[RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid ZipCode")]
        public string ZipCode { get; set; }
        public string? RoomSuite { get; set; }
        public IFormFile UploadFile { get; set; }
    }
}
