﻿using HelloDocMVC.Entity.DataModels;
using HelloDocMVC.Entity.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloDocMVC.Repository.Repository.Interface
{
    public interface IComboBox
    {
        public List<RegionComboBox> RegionComboBox();
        Task<List<CaseReasonComboBox>> CaseReasonComboBox();
        public List<Physician> ProviderbyRegion(int regionid);
        public List<Profession> Profession();
        public List<HealthProfessional> Business(int profession);
        public HealthProfessional OrderData(int vendorid);
        public Task<List<RoleComboBox>> UserRoleComboBox();
        public Task<List<RoleComboBox>> PhysicianRoleComboBox();
        public Task<List<RoleComboBox>> AdminRoleComboBox();
    }
}
