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
        Teacher GetTeacherByUserId(string userId);
        void AddTeacher(Teacher item);
    }
}
