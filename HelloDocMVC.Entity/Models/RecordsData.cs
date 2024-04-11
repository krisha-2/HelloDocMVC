using HelloDocMVC.Entity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Entity.Models
{
    public class RecordsData
    {
        public List<SearchRecords>? SearchRecords { get; set; }
        public List<User>? Users { get; set; }
        public List<BlockRequests>? BlockRequests { get; set; }
        public List<EmailLogs>? EmailLogs { get; set; }
        public List<SMSLogs>? SMSLogs { get; set; }
        // Email Logs
        public int? AccountType { get; set; }
        public string? ReceiverName { get; set; }
        // Extra Input Fields For Search Record
        public string? SearchInput { get; set; }
        public int? RegionId { get; set; }
        public int RequestType { get; set; }
        public short Status { get; set; }
        public string? PhysicianName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PatientName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        // Extra Input Fields For Patient Record
        public string? FirstName { get; set; }
        public string LastName { get; set; }
        public int UserId { get; set; }
        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
