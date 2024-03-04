using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Entity.Models
{
    public class ViewDocument
    {
        public string? RFirstName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string FileName { get; set; }
        public int RequestId { get; set; }
    }
}
