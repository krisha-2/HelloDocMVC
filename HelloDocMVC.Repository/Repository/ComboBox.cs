using HelloDocMVC.Entity.DataContext;
using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Repository.Repository.Interface;
using HelloDocMVC.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace HelloDocMVC.Repository.Repository
{
    public class ComboBox : IComboBox
    {
        private readonly HelloDocDbContext _context;
        public ComboBox(HelloDocDbContext context)
        {
            _context = context;
        }
        public  List<RegionComboBox> RegionComboBox()
        {
            return _context.Regions.Select(req => new RegionComboBox()
            {
                RegionId = req.RegionId,
                RegionName = req.Name
            })
                .ToList();
        }
        public async Task<List<CaseReasonComboBox>> CaseReasonComboBox()
        {
            return await _context.CaseTags.Select(req => new CaseReasonComboBox()
            {
                CaseReasonId = req.CaseTagId,
                CaseReasonName = req.Name
            })
                .ToListAsync();
        }
        #region Provider_By_Region
        public List<Physician> ProviderbyRegion(int regionid)
        {
            var result = _context.Physicians
                        .Where(r => r.RegionId == regionid)
                        .OrderByDescending(x => x.CreatedDate)
                        .ToList();
            return result;
        }
        #endregion
        public List<Profession> Profession()
        {
            return _context.HealthProfessionalTypes.Select(hl => new Profession()
            {
                HealthProfessionalId = hl.HealthProfessionalId,
                ProfessionName = hl.ProfessionName
            }).ToList();
        }
        public List<HealthProfessional> Business(int profession)
        {
            var result = _context.HealthProfessionals
                        .Where(r => r.Profession == profession)
                        .OrderByDescending(x => x.CreatedDate)
                        .ToList();
            return result;
        }
        public HealthProfessional OrderData(int vendorid)
        {
            var result = _context.HealthProfessionals
                        .FirstOrDefault(r => r.VendorId == vendorid);
            return result;
        }
    }
}
