using MyExams.Database.Contracts;
using MyExams.Models;
using MyExams.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Services
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IStudentClassRepository _studentClassRepository;
        private readonly IStudentRepository _studentRepository;
        public ClassService(IClassRepository classRepository, ITeacherRepository teacherRepository, IStudentClassRepository studentClassRepository, IStudentRepository studentRepository)
        {
            _classRepository = classRepository;
            _teacherRepository = teacherRepository;
            _studentClassRepository = studentClassRepository;
            _studentRepository = studentRepository;
        }
        public Class CreateNewClass(string userId, string name, string subject)
        {
            var teacher = _teacherRepository.Where(x => x.UserId == userId).FirstOrDefault();
            if (teacher != null)
            {
                var code = GenerateUniqueCode();
                var colorGenerated = GenerateColor(DateTime.Now.Millisecond * DateTime.Now.Minute - DateTime.Now.Hour);
                var classToAdd = new Models.Class()
                {
                    Name = name,
                    Subject = subject,
                    Teacher = teacher,
                    UniqueCode = code,
                    ClassColor = colorGenerated,
                     RecentUsage = DateTime.Now
                   
                };
                _classRepository.Add(classToAdd);
                _classRepository.SaveChanges();
                return classToAdd;
            }
            else
            {
                return null;
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

        public bool IsStudentOfClass(string studentUserId, string uniqueClassCode)
        {
            var classObj = _classRepository.Where(x => x.UniqueCode == uniqueClassCode).FirstOrDefault();
            var studentObj = _studentRepository.Where(x => x.UserId == studentUserId).FirstOrDefault();
            if (classObj != null && studentObj!=null)
            {
                var record = _studentClassRepository.Where(x => x.Class.Id == classObj.Id && x.Student.Id == studentObj.Id).FirstOrDefault();
                if (record != null)
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
        public IEnumerable<StudentClass> GetClassStudents(string uniqueClassCode)
        {
            var students = _studentClassRepository.GetAll().Where(x => x.Class.UniqueCode == uniqueClassCode).ToList();
            return students;
        }

        public IEnumerable<Class> GetStudentClasses(string studentUserId)
        {
            var studentClasses = _studentClassRepository.GetAll().Where(x => x.Student.UserId == studentUserId).ToList();
            var classes = new List<Class>();
            foreach (var item in studentClasses)
            {
                var foundClass = _classRepository.GetAll().Where(c => c.Id == item.Class.Id).First();
                classes.Add(foundClass);
            }
            return classes;
        }
        public IEnumerable<object> GetClassObjects<TKey>(string teacherId, Expression<Func<Models.Class, TKey>> orderBy, OrderByMethod orderByMethod)
        {
            var classesIQuer = _classRepository.Where(x => x.Teacher.UserId == teacherId).AsQueryable();
            switch (orderByMethod)
            {
                case OrderByMethod.Ascending:   classesIQuer = classesIQuer.OrderBy(orderBy);
                    break;
                case OrderByMethod.Descending:
                    classesIQuer = classesIQuer.OrderByDescending(orderBy);
                    break;
            }
                
             
            var classes = classesIQuer.ToList();
            List<object> classesInput = new List<object>();
            for (int i = 0; i < classes.Count(); i++)
            {
                classesInput.Add(new { name = classes[i].Name, studentsCount = classes[i].StudentsCount, averageMark = classes[i].AverageMark, code = classes[i].UniqueCode, subject = classes[i].Subject, color = classes[i].ClassColor });
            }
            return classesInput;
        } 
        public Class AddStudentToClass(Student student, string classCode, int noInClass)
        {
            var classRef = _classRepository.Where(x => x.UniqueCode == classCode).FirstOrDefault();
            var isAlreadyExist = _studentClassRepository.GetAll().Any(x => x.Class.UniqueCode == classCode && x.Student.Id == student.Id);
            if (classRef != null && student != null && !isAlreadyExist)
            {
                _studentClassRepository.Add(new StudentClass()
                {
                    Class = classRef,
                    Student = student,
                    NoInClass = noInClass

                });
                classRef.StudentsCount++;
                _classRepository.Update(classRef);
                _studentClassRepository.SaveChanges();
                return classRef;
            }
            return null; 

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

            if (_classRepository.Where(x => x.UniqueCode == code).FirstOrDefault() == null)
            {
                return code;
            }
            else
            {
                GenerateUniqueCode();
                return null;
            }

        }
        public string GenerateColor(int seed = 0)
        {
            string[] ColourValues = new string[] {
        "#52ae51", "#3b84b8", "#c051cd", "#cd525e", "#ec8041", "#e66741", "#5d5db0",
        "#e14da6" };
            Random rn = new Random(seed);
            return ColourValues[rn.Next(0, ColourValues.Count() - 1)];
        }
    }
}
