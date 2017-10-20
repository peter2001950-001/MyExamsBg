using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Services.Contracts
{
    public interface IStudentService
    {
        Student GetStudentByUserId(string userId);
        void AddStudent(Student item);
    }
}
