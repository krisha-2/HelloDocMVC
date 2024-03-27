using HelloDocMVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Repository.Repository.Interface
{
    public interface IProvider
    {
        public Task<List<ViewProvider>> PhysicianAll();
        public Task<bool> ChangeNotificationPhysician(Dictionary<int, bool> changedValuesDict);
    }
}
