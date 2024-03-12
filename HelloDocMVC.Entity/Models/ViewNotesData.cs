using HelloDocMVC.Entity.DataModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelloDocMVC.Entity.Models;

namespace HelloDocMVC.Entity.Models
{
    public class ViewNotesData
    {
        public int? Requestnotesid { get; set; }
        public int? Requestid { get; set; }
        public string? Strmonth { get; set; }
        public int? Intyear { get; set; }
        public int? Intdate { get; set; }
        public string? Physiciannotes { get; set; }
        public string? Adminnotes { get; set; }
        public string? Patientnotes { get; set; }
        public string? Createdby { get; set; } = null!;
        public DateTime? Createddate { get; set; }
        public string? Modifiedby { get; set; }
        public DateTime? Modifieddate { get; set; }
        public short Status { get; set; }
        public string? Ip { get; set; }
        public virtual Request Request { get; set; } = null!;
        public List<TransferNotesData> transfernotes { get; set; } = null!;
        public List<TransferNotesData> cancel { get; set; } = null!;
        public List<TransferNotesData> cancelbyphysician { get; set; } = null!;
    }
}
