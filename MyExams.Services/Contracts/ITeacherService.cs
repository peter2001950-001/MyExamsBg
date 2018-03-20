using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Services.Contracts
{
    public interface ITeacherService
    {
        IEnumerable<Teacher> GetAll();
        Teacher GetTeacherByUserId(string userId);
        void AddTeacher(Teacher item);
        bool IsTeacherOfTest(string userId, int testId);
        bool IsTeacherOfGTest(string userId, int gTestId);
    }
}
