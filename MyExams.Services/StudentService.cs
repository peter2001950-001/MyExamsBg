using MyExams.Database;
using MyExams.Database.Contracts;
using MyExams.Models;
using MyExams.Services.Contracts;

namespace MyExams.Services
{
    public class StudentService : IStudentService
    {
       private IStudentRepository _studentRepository;
        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public Student GetStudentByUserId(string userId)
        {
            return _studentRepository.GetStudentByUserId(userId);
        }
        public void AddStudent(Student item)
        {
            _studentRepository.Add(item);
            _studentRepository.SaveChanges();
        }
    }
}
