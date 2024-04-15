using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Entity.Models
{
	public class ViewRole
	{
		public int Roleid { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Status is required")]
        public short Accounttype { get; set; }
		public string Createdby { get; set; } = null!;
		public DateTime Createddate { get; set; }
		public string? Modifiedby { get; set; }
		public DateTime? Modifieddate { get; set; }
		public BitArray Isdeleted { get; set; } = null!;
		public List<Menu> Menus { get; set; }
		public class Menu
		{
			public int Menuid { get; set; }
			public string Name { get; set; }
			public string checekd { get; set; }
		}
	}
}
