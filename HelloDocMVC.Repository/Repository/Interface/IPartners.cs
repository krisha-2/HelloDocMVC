using HelloDocMVC.Entity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Repository.Repository.Interface
{
    public interface IPartners
    {
        public Task<List<HealthProfessional>> GetPartnersByProfession(int? regionId);
    }
}
