using Assignment.Entity.DataContext;
using Assignment.Entity.DataModels;
using Assignment.Entity.Models;
using Assignment.Repository.Repository.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                data.Age = vs.Age;
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
            modal.Age = (int)students.Age;
            modal.Gender = students.Gender;
            return modal;
        }
    }
}
