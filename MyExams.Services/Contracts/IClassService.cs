using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Services.Contracts
{
   public interface IClassService
    {
        Class CreateNewClass(string userId, string name, string subject);
        IEnumerable<StudentClass> GetClassStudents(string uniqueClassCode);
        IEnumerable<Class> GetStudentClasses(string studentUserId);
        Class AddStudentToClass(string userId, string classCode, int noInClass);
        string GenerateColor(int seed=0);
        bool IsTeacherOfClass(string teacherUserId, string uniqueClassCode);
        bool IsStudentOfClass(string studentUserId, string uniqueClassCode);
        IEnumerable<Class> GetAll();
    }
}
