using HelloDocMVC.Entity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Entity.Models
{
    public class ViewCloseCase
    {
        public List<ViewDocument> documentslist { get; set; } = null;
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ConfirmationNumber { get; set; }
        public int RequestID { get; set; }
        public int RequestWiseFileID { get; set; }
        public string RC_FirstName { get; set; }
        public string RC_LastName { get; set; }
        public string RC_Email { get; set; }
        public DateTime RC_Dob { get; set; }
        public string RC_PhoneNumber { get; set; }
        public int RequestClientID { get; set; }
        public List<RequestWiseFile> Files { get; set; }
    }
}
