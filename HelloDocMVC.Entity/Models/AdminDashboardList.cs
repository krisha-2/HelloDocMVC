using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Entity.Models
{
    public class AdminDashboardList
    {
        public int? RequestId { get; set; }
        public string PatientName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? IntYear { get; set; }
        public int? IntDate { get; set; }
        public string? StrMonth { get; set; }
        public int? RequestTypeId { get; set; }
        public string Requestor { get; set; }
        public DateTime RequestedDate { get; set; }
        public string PatientPhoneNumber { get; set; }
        public string RequestorPhoneNumber { get; set; }
        public string? Address { get; set; }
        public string Notes { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PatientId { get; set; }
        public int ProviderId { get; set; }
        public string? ProviderName { get; set; }
        public string RegionId { get; set; }
        public string? ADStatus { get; set; }
        public  string PhysicianName { get; set; }
        public string DateOfService { get; set; }
    }
    public class PaginatedViewModel
    {
        public List<AdminDashboardList>? adl { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchInput { get; set; }
        public int? RegionId { get; set; }
        public int? RequestType { get; set; }
        public string? Status { get; set; }
        public int NewRequest { get; set; }
        public int PendingRequest { get; set; }
        public int ActiveRequest { get; set; }
        public int ConcludeRequest { get; set; }
        public int ToCloseRequest { get; set; }
        public int UnpaidRequest { get; set; }

    }
}
