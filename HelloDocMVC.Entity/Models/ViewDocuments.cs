using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Entity.Models
{
    public class ViewDocuments
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string? ConfirmationNumber { get; set; }
        public int RequestID { get; set; }
        public class Documents
        {
            public string? Uploader { get; set; }
            public int? Status { get; set; }
            public string? Filename { get; set; }
            public DateTime? Createddate { get; set; }
            public int? RequestwisefilesId { get; set; }
            public string isDeleted { get; set; }
        }
        public List<Documents>? documentslist { get; set; } = null;
    }
}
