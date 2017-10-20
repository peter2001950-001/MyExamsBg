using MyExams.Database.Contracts;
using MyExams.Models;
using MyExams.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Services
{
    public class TeacherService:ITeacherService
    {
        private ITeacherRepository _teacherRepository;

        public TeacherService(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }
        public Teacher GetTeacherByUserId(string userId)
        {
            return _teacherRepository.GetTeacherByUserId(userId);
        }
        public void AddTeacher(Teacher item)
        {
            _teacherRepository.Add(item);
            _teacherRepository.SaveChanges();
        }
    }
}
