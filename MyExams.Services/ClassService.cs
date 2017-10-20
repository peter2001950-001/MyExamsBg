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
    public class ClassService:IClassService
    {
        private readonly IClassRepository _classRepository;
        private readonly ITeacherRepository _teacherRepository;
        public ClassService(IClassRepository classRepository, ITeacherRepository teacherRepository)
        {
            _classRepository = classRepository;
            _teacherRepository = teacherRepository;
        }
        public void CreateNewClass(string userId, string name, string subject)
        {
            var teacher = _teacherRepository.Where(x => x.UserId == userId).FirstOrDefault();
            if (teacher != null)
            {
                var code = GenerateUniqueCode();
                _classRepository.Add(new Models.Class()
                {
                    Name = name,
                    Subject = subject,
                    Teacher = teacher,
                    UniqueCode = code
                });
                _classRepository.SaveChanges();
            }
            else
            {
                throw new Exception();
            }
            
        }
        public bool IsTeacherOfClass(string teacherUserId, string uniqueClassCode)
        {
                var classObject = _classRepository.Where(x => x.UniqueCode == uniqueClassCode).FirstOrDefault();
            if (classObject != null)
            {
                if (classObject.Teacher.UserId == teacherUserId)
                {
                    return true;
                }
            }
            return false;
        }
        public IEnumerable<Class> GetAll()
        {
            return _classRepository.GetAll();
        }
        private string GenerateUniqueCode()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string code = "";
            var random = new Random();
            for (int i = 0; i < 6; i++)
            {
                code += chars[random.Next(0, 35)];
            }

            if(_classRepository.Where(x => x.UniqueCode == code).FirstOrDefault() == null)
            {
                return code;
            }
            else
            {
                GenerateUniqueCode();
                return null;
            }
            
        }
    }
}
