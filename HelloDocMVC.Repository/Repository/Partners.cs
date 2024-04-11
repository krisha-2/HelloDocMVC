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
                              Profession = asp.ProfessionName,
                              Business = Hp.VendorName,
                              Email = Hp.Email,
                              FaxNumber = Hp.FaxNumber,
                              PhoneNumber = Hp.PhoneNumber,
                              BusinessNumber = Hp.BusinessContact
                          }).ToList();
            return result;
        }
        //public bool EditPartners(HealthProfessional hp)
        //{
        //    var Data = _context.HealthProfessionals.Where(req => req.VendorId == hp.VendorId).FirstOrDefault();
        //    if (Data != null)
        //    {
        //        Data.Profession = hp.Profession;
        //        Data.VendorName = hp.VendorName;
        //        Data.Email = hp.Email;
        //        Data.FaxNumber = hp.FaxNumber;
        //        Data.PhoneNumber = hp.PhoneNumber;
        //        Data.BusinessContact = hp.BusinessContact;
        //        Data.Address = hp.Address;
        //        Data.City = hp.City;
        //        Data.Zip = hp.Zip;
        //        Data.State = hp.State;
        //        _context.HealthProfessionals.Update(Data);
        //        _context.SaveChanges();
        //        return true;
        //    }
        //    else
        //    {
        //        var data = new HealthProfessional();

        //        data.Profession = hp.Profession;
        //        data.VendorName = hp.VendorName;
        //        data.Email = hp.Email;
        //        data.FaxNumber = hp.FaxNumber;
        //        data.PhoneNumber = hp.PhoneNumber;
        //        data.BusinessContact = hp.BusinessContact;
        //        data.Address = hp.Address;
        //        data.City = hp.City;
        //        data.Zip = hp.Zip;
        //        data.State = hp.State;
        //        _context.HealthProfessionals.Add(data);
        //        _context.SaveChanges();
        //        return true;
        //    }
        //}
        //public bool AddBusiness(HealthProfessional hp)
        //{
        //    var Data = _context.HealthProfessionals.Where(req => req.VendorId == hp.VendorId).FirstOrDefault();
        //    if (Data != null)
        //    {
        //        Data.Profession = hp.Profession;
        //        Data.VendorName = hp.VendorName;
        //        Data.Email = hp.Email;
        //        Data.FaxNumber = hp.FaxNumber;
        //        Data.PhoneNumber = hp.PhoneNumber;
        //        Data.BusinessContact = hp.BusinessContact;
        //        Data.Address = hp.Address;
        //        Data.City = hp.City;
        //        Data.Zip = hp.Zip;
        //        Data.State = hp.State;
        //        _context.HealthProfessionals.Update(Data);
        //        _context.SaveChanges();
        //        return true;
        //    }
        //    else
        //    {
        //        var data = new HealthProfessional();

        //        data.Profession = hp.Profession;
        //        data.VendorName = hp.VendorName;
        //        data.Email = hp.Email;
        //        data.FaxNumber = hp.FaxNumber;
        //        data.PhoneNumber = hp.PhoneNumber;
        //        data.BusinessContact = hp.BusinessContact;
        //        data.Address = hp.Address;
        //        data.City = hp.City;
        //        data.Zip = hp.Zip;
        //        data.State = hp.State;
        //        data.IsDeleted = new BitArray(1);
        //        data.CreatedDate = DateTime.Now;
        //        _context.HealthProfessionals.Add(data);
        //        _context.SaveChanges();
        //        return true;
        //    }
        //}
        public bool DeleteBusiness(int vendorId)
        {
            HealthProfessional hp = _context.HealthProfessionals.Where(x => x.VendorId == vendorId).FirstOrDefault();
            hp.IsDeleted[0] = true;
            _context.HealthProfessionals.Update(hp);
            _context.SaveChanges();
            return true;
        }
    }
}
