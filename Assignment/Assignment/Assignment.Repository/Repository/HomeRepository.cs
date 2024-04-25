using Assignment.Entity.DataContext;
using Assignment.Entity.DataModels;
using Assignment.Entity.Models;
using Assignment.Repository.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Assignment.Repository.Repository
{
    public class HomeRepository : IHomeRepository
    {
        private readonly AssignmentDbContext _context;
        public HomeRepository(AssignmentDbContext context)
        {
            _context = context;
        }
        public List<ViewStudents> ViewTableRequest()
        {
            var item = (from C in _context.Courses
                                      join S in _context.Students
                                      on C.Id equals S.CourseId into CS
                                      from S in CS.DefaultIfEmpty()
                                      where S.IsDeleted == new BitArray(1)
                        select new ViewStudents
                                      {
                                          StudentId = S.Id,
                                          Id = S.Id,
                                          FirstName = S.FirstName,
                                          LastName = S.LastName,
                                          Email = S.Email,
                                          Age = (int)S.Age,
                                          Gender = S.Gender,
                                          Course = S.Course,
                                          Grade = S.Grade

                                      }).ToList();
            return item;
        }
        public bool DeleteBusiness(int Id)
        {
            try
            {
                var data = _context.Students.FirstOrDefault(v => v.Id == Id);
                data.IsDeleted = new BitArray(1);
                data.IsDeleted[0] = true;
                _context.Students.Update(data);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool AddStudent(ViewStudents vs)
        {
            var Data = _context.Students.Where(req => req.Id == vs.Id).FirstOrDefault();
            
                var data = new Student();
                data.Id = vs.StudentId;
                data.FirstName = vs.FirstName;
                data.Email = vs.Email;
                data.LastName = vs.LastName;
                data.Age = DateTime.Now.Year-vs.Dob.Year;
                data.Gender = vs.Gender;
                data.Course = vs.Course;
                data.Grade = vs.Grade;
                data.IsDeleted = new BitArray(1);
                data.IsDeleted[0] = false;

            _context.Students.Add(data);
                _context.SaveChanges();
                return true;
        }
        public ViewStudents StudentData(int Id)
        {
            ViewStudents modal = new ViewStudents();
            var students = _context.Students.FirstOrDefault();

            modal.FirstName = students.FirstName;
            modal.LastName = students.LastName;
            modal.Email = students.Email;
            //modal.Dob = students.A;
            modal.Gender = students.Gender;
            modal.Course = students.Course;
            return modal;
        }
        public async Task<bool> EditData(ViewStudents viewdata)
        {
            
            var DataForChange = await _context.Students.Where(W => W.Id == viewdata.Id).FirstOrDefaultAsync();
                   
                DataForChange.FirstName = viewdata.FirstName;
                DataForChange.LastName = viewdata.LastName;
                DataForChange.Email = viewdata.Email;
                //DataForChange.Age = viewdata.Age;
                DataForChange.Age = DateTime.Now.Year - viewdata.Dob.Year;
                DataForChange.Gender = viewdata.Gender;
                DataForChange.Course = viewdata.Course;
                _context.Students.Update(DataForChange);
                _context.SaveChanges();
                return true;
        }
    }
}
