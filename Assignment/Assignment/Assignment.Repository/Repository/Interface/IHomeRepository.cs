using Assignment.Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Repository.Repository.Interface
{
    public interface IHomeRepository
    {
        public List<ViewStudents> ViewTableRequest();
        public bool DeleteBusiness(int Id);
        public bool AddStudent(ViewStudents vs);
        public ViewStudents StudentData(int Id);
        public Task<bool> EditData(ViewStudents viewdata);
    }
}
