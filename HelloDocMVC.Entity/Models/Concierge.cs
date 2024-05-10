using System.ComponentModel.DataAnnotations;
namespace HelloDocMVC.Entity.Models
{
    public class Concierge
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName is required")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "FirstName is required")]
        public string First_Name { get; set; }
        [Required(ErrorMessage = "LastName is required")]
        public string? Last_Name { get; set; }
        public string RFirstName { get; set; }
        public string? RLastName { get; set; }
        public string RcFirstName { get; set; }
        public string? RcLastName { get; set; }
        [Required(ErrorMessage = "PhoneNumber is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile Number must be of 10 digits!")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Mobile Number must contain digits!")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "PhoneNumber is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile Number must be of 10 digits!")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Mobile Number must contain digits!")]
        public string Phone_Number { get; set; }
        public string Mobile { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please Enter Valid Email Address!")]
        public string E_mail { get; set; }
        public string? RelationWithPatient { get; set; }
        public string? RelationName { get; set; }
        public string? PatientDetails { get; set; }
        public string? Symptoms { get; set; }
        public DateTime DOB { get; set; }
        public string? Contact { get; set; }
        [Required(ErrorMessage = "Street is required")]
        public string? Street { get; set; }
        [Required(ErrorMessage = "City is required")]
        public string? City { get; set; }
        [Required(ErrorMessage = "State is required")]
        public string? State { get; set; }
        [Required(ErrorMessage = "ZipCode is Required")]
        //[RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid ZipCode")]
        public string ZipCode { get; set; }
        public string? RoomSuite { get; set; }
    }
}
