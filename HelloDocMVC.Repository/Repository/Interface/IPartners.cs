using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Repository.Repository.Interface
{
    public interface IPartners
    {
        public List<PartnersData> GetPartnersByProfession(string searchValue, int Profession);
        //public bool EditPartners(HealthProfessional hp);
        //public bool AddBusiness(HealthProfessional hp);
        //public bool DeleteBusiness(int vendorId);
        public Task<PartnersData> BusinessById(int? VendorId);
        public bool AddEditBusiness(PartnersData data);
        public bool DeleteVendor(int? VendorId);
    }
}
