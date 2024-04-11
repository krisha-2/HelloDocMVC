using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Entity.Models
{
    public class SMSLogs
    {
        public int SmsLogId { get; set; }
        public string Recipient { get; set; }
        public string? SmsTemplate { get; set; }
        public string MobileNumber { get; set; } = null!;
        public string? ConfirmatioNumber { get; set; }
        public int? RoleId { get; set; }
        public int? AdminId { get; set; }
        public int? RequestId { get; set; }
        public int? PhysicianId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime SentDate { get; set; }
        public BitArray? IsSmsSent { get; set; }
        public int SentTries { get; set; }
        public int? Action { get; set; }
    }
}
