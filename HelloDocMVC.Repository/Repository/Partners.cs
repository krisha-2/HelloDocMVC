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
        public PartnersData GetPartnersByProfession(string searchValue, int Profession, PartnersData pd)
        {
            List<PartnersData> result = (from Hp in _context.HealthProfessionals
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
            if (pd.IsAscending == true)
            {
                result = pd.SortedColumn switch
                {
                    "Profession" => result.OrderBy(x => x.Profession).ToList(),
                    "Business" => result.OrderBy(x => x.Business).ToList(),
                    "Email" => result.OrderBy(x => x.Email).ToList(),
                    _ => result.OrderBy(x => x.Business).ToList()
                };
            }
            else
            {
                result = pd.SortedColumn switch
                {
                    "PhysicianName" => result.OrderByDescending(x => x.Profession).ToList(),
                    "Business" => result.OrderByDescending(x => x.Business).ToList(),
                    "Email" => result.OrderByDescending(x => x.Email).ToList(),
                    _ => result.OrderByDescending(x => x.Business).ToList()
                };
            }
            int totalItemCount = result.Count;
            int totalPages = (int)Math.Ceiling(totalItemCount / (double)pd.PageSize);
            List<PartnersData> list = result.Skip((pd.CurrentPage - 1) * pd.PageSize).Take(pd.PageSize).ToList();

            PartnersData model = new()
            {
                PD = list,
                CurrentPage = pd.CurrentPage,
                TotalPages = totalPages,
                PageSize = pd.PageSize,
                IsAscending = pd.IsAscending,
                SortedColumn = pd.SortedColumn
            };

            return model;
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
        //public bool DeleteBusiness(int vendorId)
        //{
        //    HealthProfessional hp = _context.HealthProfessionals.Where(x => x.VendorId == vendorId).FirstOrDefault();
        //    hp.IsDeleted[0] = true;
        //    _context.HealthProfessionals.Update(hp);
        //    _context.SaveChanges();
        //    return true;
        //}
        public HealthProfessional EditPartners(int VendorId)
        {
            var result = _context.HealthProfessionals.Where(Req => Req.VendorId == VendorId).FirstOrDefault();
            return result;
        }
        public bool EditPartnersData(HealthProfessional hp)
        {
            var Data = _context.HealthProfessionals.Where(req => req.VendorId == hp.VendorId).FirstOrDefault();
            if (Data != null)
            {
                Data.Profession = hp.Profession;
                Data.VendorName = hp.VendorName;
                Data.Email = hp.Email;
                Data.FaxNumber = hp.FaxNumber;
                Data.PhoneNumber = hp.PhoneNumber;
                Data.BusinessContact = hp.BusinessContact;
                Data.Address = hp.Address;
                Data.City = hp.City;
                Data.Zip = hp.Zip;
                Data.State = hp.State;
                _context.HealthProfessionals.Update(Data);
                _context.SaveChanges();
                return true;
            }
            else
            {
                var data = new HealthProfessional();
                data.Profession = hp.Profession;
                data.VendorName = hp.VendorName;
                data.Email = hp.Email;
                data.FaxNumber = hp.FaxNumber;
                data.PhoneNumber = hp.PhoneNumber;
                data.BusinessContact = hp.BusinessContact;
                data.Address = hp.Address;
                data.City = hp.City;
                data.Zip = hp.Zip;
                data.State = hp.State;
                _context.HealthProfessionals.Add(data);
                _context.SaveChanges();
                return true;
            }
        }
        public bool DeleteBusiness(int VendorId)
        {
            HealthProfessional r = _context.HealthProfessionals.Where(x => x.VendorId == VendorId).FirstOrDefault();
            r.IsDeleted[0] = true;
            r.ModifiedDate = DateTime.Now;
            _context.HealthProfessionals.Update(r);
            _context.SaveChanges();
            return true;
        }
    }
}
