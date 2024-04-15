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
        public PartnersData GetPartnersByProfession(string searchValue, int Profession, PartnersData pd);
        //public bool EditPartners(HealthProfessional hp);
        //public bool AddBusiness(HealthProfessional hp);
        //public bool DeleteBusiness(int vendorId);
        public HealthProfessional EditPartners(int VendorId);
        public bool EditPartnersData(HealthProfessional hp);
        public bool DeleteBusiness(int VendorId);
    }
}
