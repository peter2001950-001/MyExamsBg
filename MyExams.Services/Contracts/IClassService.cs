using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Services.Contracts
{
   public interface IClassService
    {
        Class CreateNewClass(string userId, string name, string subject);
        IEnumerable<StudentClass> GetClassStudents(string uniqueClassCode);
        IEnumerable<Class> GetStudentClasses(string studentUserId);
        IEnumerable<object> GetClassObjects<TKey>(string teacherId, Expression<Func<Models.Class, TKey>> orderBy, OrderByMethod orderByMethod);
        Class AddStudentToClass(Student student, string classCode, int noInClass);
        string GenerateColor(int seed=0);
        bool IsTeacherOfClass(string teacherUserId, string uniqueClassCode);
        bool IsStudentOfClass(string studentUserId, string uniqueClassCode);
        IEnumerable<Class> GetAll();
    }
}
