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
        void CreateNewClass(string userId, string name, string subject);
        bool IsTeacherOfClass(string teacherUserId, string uniqueClassCode);
        IEnumerable<Class> GetAll();
    }
}
