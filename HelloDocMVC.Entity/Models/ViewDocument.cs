using HelloDocMVC.Entity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Entity.Models
{
    public class ViewDocument
    {
        public string FileName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UserName { get; set; }
        public string RFirstName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RequestId { get; set; }
        public string ConfirmationNumber { get; set; }
        //public string Filename { get; set; }
        public string isDeleted { get; set; }
        public int RequestWiseFileId { get; set; }
        public List<RequestWiseFile> Files { get; set; }
    }
}

