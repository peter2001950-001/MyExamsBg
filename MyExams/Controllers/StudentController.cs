using Microsoft.AspNet.Identity;
using MyExams.Models;
using MyExams.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyExams.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : ApplicationBaseController
    {
        private readonly IClassService _classService;
        private readonly ITeacherService _teacherService;
        public StudentController(IClassService classService, ITeacherService teacherService)
        {
            _classService = classService;
            _teacherService = teacherService;
        }
        // GET: Student
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetClasses()
        {
            var userId = User.Identity.GetUserId();
           var classes =  _classService.GetStudentClasses(userId).ToList();
            var classesOutput = new object[classes.Count()];
            for (int i = 0; i < classes.Count(); i++)
            {
                classesOutput[i] = new { name = classes[i].Name, studentsCount = classes[i].StudentsCount, code = classes[i].UniqueCode, subject = classes[i].Subject, color = classes[i].ClassColor, teacher = classes[i].Teacher.FirstName + " " + classes[i].Teacher.LastName };
            }
            return Json(new { status = "OK", classes = classesOutput }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult JoinClass(JoinClassViewModel model)
        {
            int noInClass = 0;
            if(int.TryParse(model.noInClass, out noInClass))
            {
                var classRef = _classService.AddStudentToClass(User.Identity.GetUserId(), model.ClassCode, noInClass);
               if (classRef!=null)
                {
                    return Json(new { status = "OK", classCode = classRef.UniqueCode });
                }
            }
            return Json(new { status = "ERR" });
        }
        public ActionResult Class(string id)
        {
            var userId = User.Identity.GetUserId();
            if (_classService.IsStudentOfClass(userId, id))
            {
                var foundClass = _classService.GetAll().Where(x => x.UniqueCode == id).FirstOrDefault();
                var teacher = _teacherService.GetAll().Where(x => x.Id == foundClass.Teacher.Id).FirstOrDefault();
                if (teacher != null)
                {
                    var viewModel = new DisplayClassStudentViewModel()
                    {
                        ClassName = foundClass.Name,
                        Subject = foundClass.Subject,
                        StudentsCount = foundClass.StudentsCount,
                        UniqueCode = foundClass.UniqueCode,
                        ClassColor = foundClass.ClassColor,
                        TeacherName = teacher.FirstName + " " + teacher.LastName
                    };
                    return View(viewModel);
                }
               
            }
            return RedirectToAction("Index");
        }

        public JsonResult GetStudents(string uniqueCode)
        {
            var userId = User.Identity.GetUserId();
            if (_classService.IsStudentOfClass(userId, uniqueCode))
            {
                var students = _classService.GetClassStudents(uniqueCode).OrderBy(x => x.NoInClass).ToList();
                object[] studentsOutput = new object[students.Count];
                for (int i = 0; i < students.Count; i++)
                {
                    studentsOutput[i] = new { firstName = students[i].Student.FirstName, lastName = students[i].Student.LastName, noInClass = students[i].NoInClass };
                }
                return Json(new { students = studentsOutput, tests = "", status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "ERR" }, JsonRequestBehavior.AllowGet);
        }
    }
}