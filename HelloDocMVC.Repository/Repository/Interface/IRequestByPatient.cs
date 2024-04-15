using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Repository.Repository.Interface
{
    public interface IRequestByPatient
    {
        public Patient viewMeData(int id);
        public void meRequset(Patient viewPatientReq);
        public void elseRequset(FamilyFriend viewFamilyReq);
    }
}
