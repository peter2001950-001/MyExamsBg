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
        private readonly ITeacherRepository _teacherRepository;
        private readonly ITestRepository _testRepository;
        private readonly IGTestRepository _gTestRepository;
        public TeacherService(ITeacherRepository teacherRepository, ITestRepository testRepository, IGTestRepository gTestRepository)
        {
            _teacherRepository = teacherRepository;
            _testRepository = testRepository;
            _gTestRepository = gTestRepository;
        }
        public Teacher GetTeacherByUserId(string userId)
        {
            return _teacherRepository.GetTeacherByUserId(userId);
        }
        public IEnumerable<Teacher> GetAll()
        {
            return _teacherRepository.GetAll();
        }
        public void AddTeacher(Teacher item)
        {
            _teacherRepository.Add(item);
            _teacherRepository.SaveChanges();
        }
        public bool IsTeacherOfTest(string userId, int testId)
        {
            var teacher = GetTeacherByUserId(userId);
            if (teacher != null)
            {
                var test = _testRepository.GetAll().FirstOrDefault(x => x.Id == testId);
                if (test != null)
                {
                    if(test.Teacher.Id == teacher.Id)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool IsTeacherOfGTest(string userId, int gTestId)
        {
            var teacher = GetTeacherByUserId(userId);
            if (teacher != null)
            {
                var gTest = _gTestRepository.Include(x => x.Teacher).Where(x => x.Id == gTestId).FirstOrDefault();
                if (gTest != null)
                {
                    if (gTest.Teacher.Id == teacher.Id)
                        return true;
                }
            }
            return false;
        }
    }
}
