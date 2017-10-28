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
    [Authorize(Roles ="Teacher")]
    public class TeacherController : ApplicationBaseController
    {
        private readonly IClassService _classService;
        public TeacherController(IClassService classService)
        {
            _classService = classService;
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
        public ActionResult Class(string id)
        {
            var userId = User.Identity.GetUserId();
            if (_classService.IsTeacherOfClass(userId, id))
            {
                var foundClass = _classService.GetAll().Where(x => x.UniqueCode == id).FirstOrDefault();
                var viewModel = new DisplayClassViewModel()
                {
                    ClassName = foundClass.Name,
                    Subject = foundClass.Subject
                };
                return View(viewModel);
            }
            return RedirectToAction("classes");
        }
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewClass(NewClassViewModel model)
        {
            var userId = User.Identity.GetUserId();
            _classService.CreateNewClass(userId, model.Name, model.Subject);
            return Json(new { });
        }
        [HttpGet]
        public ActionResult GetClasses()
        {
            var userId = User.Identity.GetUserId();
            var classes = _classService.GetAll().Where(x => x.Teacher.UserId == userId).ToList();
            object[] clasesInput = new object[classes.Count()];
            for (int i = 0; i < classes.Count(); i++)
            {
                clasesInput[i] = new { name = classes[i].Name, studentsCount = classes[i].StudentsCount, averageMark = classes[i].AverageMark, code = classes[i].UniqueCode };
            }
            return Json(new {classes = clasesInput, status = "OK" }, JsonRequestBehavior.AllowGet);
        }
             
    }
}