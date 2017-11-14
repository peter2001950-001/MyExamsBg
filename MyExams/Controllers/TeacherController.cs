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
    [Authorize(Roles = "Teacher")]
    public class TeacherController : ApplicationBaseController
    {
        private readonly IClassService _classService;
        private readonly IStudentService _studentService;
        public TeacherController(IClassService classService, IStudentService studentService)
        {
            _classService = classService;
            _studentService = studentService;
        }
        // GET: Teacher
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Classes()
        {
            return View();
        }
        public ActionResult Tests()
        {
            return View();
        }
        public ActionResult TestDesign()
        {
            return View();
        }

        public ActionResult Class(string id)
        {
            var userId = User.Identity.GetUserId();
            if (_classService.IsTeacherOfClass(userId, id))
                    {
                        var foundClass = _classService.GetAll().Where(x => x.UniqueCode == id).FirstOrDefault();
                        var viewModel = new DisplayClassTeacherViewModel()
                        {
                            ClassName = foundClass.Name,
                            Subject = foundClass.Subject,
                            AverageMark = foundClass.AverageMark,
                            FirstMessageShowed = foundClass.FirstMessageShowed,
                            StudentsCount = foundClass.StudentsCount,
                            UniqueCode = foundClass.UniqueCode,
                            ClassColor = foundClass.ClassColor
                        };
                        return View(viewModel);
                    }
             return RedirectToAction("classes");

        }
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewClass(NewClassViewModel model)
        {
            var userId = User.Identity.GetUserId();
            var addedClass = _classService.CreateNewClass(userId, model.Name, model.Subject);
            if (addedClass != null)
            {
                return Json(new { code = addedClass.UniqueCode, status = "OK" });
            }
            return Json(new { status = "ERR" });

        }
        [HttpGet]
        public ActionResult GetClasses()
        {
            var userId = User.Identity.GetUserId();
            var classes = _classService.GetAll().Where(x => x.Teacher.UserId == userId).OrderBy(x => x.Name).ToList();
            object[] clasesInput = new object[classes.Count()];
            for (int i = 0; i < classes.Count(); i++)
            {
                if(classes[i].ClassColor == null)
                {
                   classes[i].ClassColor =  _classService.GenerateColor(DateTime.Now.Millisecond+  i);
                }
                clasesInput[i] = new { name = classes[i].Name, studentsCount = classes[i].StudentsCount, averageMark = classes[i].AverageMark, code = classes[i].UniqueCode, subject = classes[i].Subject, color = classes[i].ClassColor };
            }
            return Json(new { classes = clasesInput, status = "OK" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStudents(string uniqueCode)
        {
             var userId = User.Identity.GetUserId();
            if (_classService.IsTeacherOfClass(userId, uniqueCode))
            {
                var students = _classService.GetClassStudents(uniqueCode).OrderBy(x=>x.NoInClass).ToList();
                object[] studentsOutput = new object[students.Count];
                for (int i = 0; i < students.Count; i++)
                {
                    studentsOutput[i] = new { firstName = students[i].Student.FirstName, lastName=students[i].Student.LastName, noInClass = students[i].NoInClass  };
                }
                return Json(new { students = studentsOutput, tests = "", status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "ERR" }, JsonRequestBehavior.AllowGet);
         
        }

    }
}