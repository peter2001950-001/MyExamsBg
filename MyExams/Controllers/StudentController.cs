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
        // GET: Student
        public ActionResult Index()
        {
            return View();
        }
    }
}