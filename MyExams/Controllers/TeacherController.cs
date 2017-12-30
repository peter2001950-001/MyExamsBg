using Microsoft.AspNet.Identity;
using MyExams.Models;
using MyExams.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MyExams.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : ApplicationBaseController
    {
        private readonly IClassService _classService;
        private readonly IStudentService _studentService;
        private readonly ITestService _testService;
        private readonly ITeacherService _teacherService;
        private readonly ISectionService _sectionService;
        private readonly IQuestionService _questionService;
        private readonly IAnswerService _answerService;
        public TeacherController(IClassService classService, IStudentService studentService, ITestService testService, ITeacherService teacherService, ISectionService sectionService, IQuestionService questionService, IAnswerService  answerService)
        {
            _classService = classService;
            _studentService = studentService;
            _testService = testService;
            _teacherService = teacherService;
            _sectionService = sectionService;
            _questionService = questionService;
            _answerService = answerService;
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
        [ValidateAntiForgeryToken]
        public ActionResult CreateTest()
        {
            var userId = User.Identity.GetUserId();
            var teacher = _teacherService.GetTeacherByUserId(userId);
            Test test = new Test()
            {
                LastUpdated = DateTime.Now,
                Teacher = teacher

            };
            _testService.AddNewTest(test);

            return Json(new {status = "OK", id=test.UniqueNumber });
        }
        public ActionResult TestDesign(string id)
        {
            var test = _testService.GetTestByUniqueNumber(id);
            ViewBag.id = id;
            ViewBag.title = test.TestTitle;
            return View();
        }
        public ActionResult ElementUpdate(string type, string action, int index, string testUniqueCode,int sectionId=0, int questionId=0,  int questionType = 0)
        {
            var test = _testService.GetTestByUniqueNumber(testUniqueCode);
            if (test != null)
            {
                switch (type)
                {
                    case "section":
                        if (action == "added")
                        {
                            Section section = new Section()
                            {
                                Active = true,
                                IsInUse = false,
                                OrderNo = index,
                                SectionTitle = "",
                                Test = test
                            };
                            _sectionService.AddSection(section);
                        return Json(new { status = "OK" });
                        }
                        break;
                    case "question":
                        if (action == "added")
                        {
                            var section = _sectionService.GetAllSectionsByTestId(test.Id).Where(x => x.OrderNo == sectionId).FirstOrDefault();
                            if (section != null)
                            {
                                Question question = new Question()
                                {
                                    Active = true,
                                    OrderNo = index,
                                    IsInUse = false,
                                    QuestionType = (QuestionType)questionType,
                                    Text = "",
                                     Section = section
                                      
                                };
                                _questionService.AddQuestion(question);
                                return Json(new { status = "OK" });
                            }
                            return Json(new { status = "ERR1" });

                        }
                       
                        break;
                    case "option":
                        if (action == "added")
                        {
                            var questionObj = _questionService.GetAllQuestionsBy(test.Id, sectionId).FirstOrDefault(x => x.OrderNo == questionId);
                            if (questionObj != null)
                            {
                                Answer answer = new Answer()
                                {
                                    Active = true,
                                    IsInUse = false,
                                    Text = "",
                                    IsCorrect = false,
                                    OrderNo = index,
                                    Question = questionObj
                                };
                                _answerService.AddAnswer(answer);
                                return Json(new { status = "OK" });
                            }
                            return Json(new { status = "ERR1" });
                        }
                        break;
                    default:
                        break;
                }
                return Json(new { status = "ERR2" });
            }
            return Json(new { status = "ERR3" });
        }

        public ActionResult QuestionUpdate(string data)
        {
            var obj = new JavaScriptSerializer().Deserialize<ParseQuestionClass>(data);
            var test = _testService.GetTestByUniqueNumber(obj.testCode);
            if (test != null)
            {
                var section = _sectionService.GetAllSectionsByTestId(test.Id).Where(x => x.OrderNo == obj.sectionId).FirstOrDefault(x => x.Active == true);
                if (section != null)
                {
                    var question = _questionService.GetAllQuestionsBy(test.Id, obj.sectionId).Where(x => x.OrderNo == obj.question.id).FirstOrDefault(x => x.Active == true);
                    if (question != null)
                    {
                        question.Text = obj.question.text;
                        if(question.QuestionType == QuestionType.Choice)
                        {
                            var options = _answerService.GetAllBy(test.Id, obj.sectionId, obj.question.id).Where(x=>x.Active == true);
                            foreach (var item in options)
                            {
                                var option = obj.question.options.Where(x => x.id == item.OrderNo).FirstOrDefault();
                                if (option != null)
                                {
                                    item.Text = option.text;
                                    item.IsCorrect = option.isCorrect;
                                }
                                _answerService.Update(item);
                            }
                        }else if(question.QuestionType == QuestionType.Text)
                        {
                            string[] sizes = { "Кратък", "Среден", "Дълъг" };
                            var index = Array.IndexOf(sizes, obj.question.selectedAnswerSize);
                            question.QuestionAnswerSize = (QuestionAnswerSize)index;
                            question.CorrectAnswer = obj.question.correctAnswer;
                        }
                        _questionService.Update(question);
                        return Json(new { status = "OK" });
                    }
                    return Json(new { status = "ERR1" });
                }
                return Json(new { status = "ERR2" });
            }
            return Json(new { status = "ERR3" });
        }
        public class ParseQuestionClass
        {
            public string testCode { get; set; }
            public int sectionId { get; set; }
            public Question question { get; set; }
            public class Question
            {
                public int id { get; set; }
                public string text { get; set; }
                public List<Option> options { get; set; }
                public string correctAnswer { get; set; }
                public string selectedAnswerSize { get; set; }
                public class Option
                {
                    public int id { get; set; }
                    public string text { get; set; }
                    public bool isCorrect { get; set; }
                }
            }
        }

        public ActionResult GetTest(string testUniqueCode)
        {
            var test = _testService.GetTestByUniqueNumber(testUniqueCode);
            var teacher = _teacherService.GetTeacherByUserId(User.Identity.GetUserId());
            if (test !=null && teacher.Id == test.Teacher.Id)
            {
                var testObj = _testService.GetTestByUniqueNumber(testUniqueCode);
                if (testObj != null)
                {
                    var sections = _sectionService.GetAllSectionsByTestId(testObj.Id);
                    List<object> sectionsList = new List<object>();
                    
                    foreach (var section in sections)
                    {
                        if (section.Active) {
                            var questionsList = new List<object>();

                            var questions = _questionService.GetAllQuestionsBy(test.Id, section.OrderNo);
                            foreach (var question in questions)
                            {
                                if (question.Active) {
                                    var optionsList = new List<object>();
                                    if (question.QuestionType == QuestionType.Choice)
                                    {
                                        var options = _answerService.GetAllBy(test.Id, section.OrderNo, question.OrderNo);
                                        foreach (var option in options)
                                        {
                                            if (option.Active)
                                            {
                                                optionsList.Add(new { id = option.OrderNo, text = option.Text, isCorrect = option.IsCorrect });
                                            }

                                        }
                                        questionsList.Add(new { id = question.OrderNo, text = question.Text, type = (int)question.QuestionType, options = optionsList });
                                    } else if (question.QuestionType == QuestionType.Text)
                                    {
                                        questionsList.Add(new { id = question.OrderNo, text = question.Text, type = (int)question.QuestionType, answerSize = (int)question.QuestionAnswerSize, correctAnswer = question.CorrectAnswer });
                                    }
                                }

                            }
                            sectionsList.Add(new { id = section.OrderNo, text = section.SectionTitle, questions = questionsList});
                                }
                    }
                    return Json(new { status = "OK", sections = sectionsList });
                }
                return Json(new { status = "ERR1" });
            }
            return Json(new { status = "ERR2" });
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