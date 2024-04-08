using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models;
using HelloDocMVC.Repository.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Repository.Repository
{
    public class Partners : IPartners
    {
        private readonly HelloDocDbContext _context;

        public Partners(HelloDocDbContext context)
        {
            _context = context;
        }
        //#region GetPartnersByProfession
        //public async Task<List<HealthProfessional>> GetPartnersByProfession(int? regionId)
        //{
        //    List<HealthProfessional> pl = await (from r in _context.HealthProfessionals
        //                                         where r.IsDeleted == new BitArray(1) && (!regionId.HasValue || r.RegionId == regionId)
        //                                         select r)
        //                                .ToListAsync();
        //    return pl;
        //}
        //#endregion
        public List<PartnersData> GetPartnersByProfession(string searchValue, int Profession)
        {
            var result = (from Hp in _context.HealthProfessionals
                          join Hpt in _context.HealthProfessionalTypes
                          on Hp.Profession equals Hpt.HealthProfessionalId into AdminGroup
                          from asp in AdminGroup.DefaultIfEmpty()
                          where (searchValue == null || Hp.VendorName.Contains(searchValue))
                             && (Profession == 0 || Hp.Profession == Profession)
                          select new PartnersData
                          {
                              Profession = Hp.VendorName,
                              Business = asp.ProfessionName,
                              Email = Hp.Email,
                              FaxNumber = Hp.FaxNumber,
                              PhoneNumber = Hp.PhoneNumber,
                              BusinessNumber = Hp.BusinessContact
                          }).ToList();
            return result;
        }
    }
}
