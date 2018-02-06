using Microsoft.AspNet.Identity;
using MyExams.Models;
using MyExams.Services.Contracts;
using MyExams.TestProcessing.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly ITestGeneration _testGeneration;
        public TeacherController(IClassService classService, IStudentService studentService, ITestService testService, ITeacherService teacherService, ISectionService sectionService, IQuestionService questionService, IAnswerService  answerService, ITestGeneration testGeneration)
        {
            _classService = classService;
            _studentService = studentService;
            _testService = testService;
            _teacherService = teacherService;
            _sectionService = sectionService;
            _questionService = questionService;
            _answerService = answerService;
            _testGeneration = testGeneration;
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
        public ActionResult TestNameUpdate(string testUniqueCode, string name)
        {
            var test = _testService.GetTestByUniqueNumber(testUniqueCode);

            if (test != null)
            {
                if (_teacherService.IsTeacherOfTest(User.Identity.GetUserId(), test.Id))
                {
                    test.TestTitle = name;
                    _testService.Update();
                    return Json(new { status = "OK" });
                }
                return Json(new { status = "ERR1" });
            }
            return Json(new { status = "ERR2" });
        }
        public ActionResult ElementUpdate(string type, string action, int index, string testUniqueCode,int sectionId=0, int questionId=0,  int questionType = 0)
        {
            var test = _testService.GetTestByUniqueNumber(testUniqueCode);
            if (test != null)
            {
                if (_teacherService.IsTeacherOfTest(User.Identity.GetUserId(), test.Id))
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
                            else if (action == "deleted")
                            {
                                var sections = _sectionService.GetAllSectionsByTestId(test.Id);

                                var sectionToRemove = sections.FirstOrDefault(x => x.OrderNo == index);
                                var questionsToRemove = _questionService.GetAllQuestionsBy(test.Id, sectionToRemove.OrderNo);
                                foreach (var item in questionsToRemove)
                                {
                                    if (item.QuestionType == QuestionType.Choice)
                                    {
                                        var options = _answerService.GetAllBy(test.Id, sectionToRemove.OrderNo, item.OrderNo);
                                        foreach (var option in options)
                                        {
                                            _answerService.RemoveAnswer(option);
                                        }
                                    }
                                    _questionService.RemoveQuestion(item);
                                }
                                _sectionService.RemoveSection(sectionToRemove);

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
                                        Section = section,
                                        MixupAnswers = true

                                    };
                                    _questionService.AddQuestion(question);
                                    return Json(new { status = "OK" });
                                }
                                return Json(new { status = "ERR1" });

                            }
                            else if (action == "deleted")
                            {
                                var questions = _questionService.GetAllQuestionsBy(test.Id, sectionId);
                                var questionToRemove = questions.FirstOrDefault(x => x.OrderNo == index);
                                if (questionToRemove.QuestionType == QuestionType.Choice)
                                {
                                    var options = _answerService.GetAllBy(test.Id, sectionId, questionToRemove.OrderNo);
                                    foreach (var item in options)
                                    {
                                        _answerService.RemoveAnswer(item);
                                    }
                                }
                                _questionService.RemoveQuestion(questionToRemove);
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
                            else if (action == "deleted")
                            {
                                var option = _answerService.GetAllBy(test.Id, sectionId, questionId).FirstOrDefault(x => x.OrderNo == index);
                                _answerService.RemoveAnswer(option);
                            }
                            break;
                        default:
                            break;
                    }
                    return Json(new { status = "ERR2" });
                }
                return Json(new { status = "ERR3" });
            }
            return Json(new { status = "ERR4" });

        }

        public ActionResult QuestionUpdate(string data)
        {
            var obj = new JavaScriptSerializer().Deserialize<ParseQuestionClass>(data);
            var test = _testService.GetTestByUniqueNumber(obj.testCode);
            if (test != null)
            {
                if (_teacherService.IsTeacherOfTest(User.Identity.GetUserId(), test.Id))
                {
                    var section = _sectionService.GetAllSectionsByTestId(test.Id).Where(x => x.OrderNo == obj.sectionId).FirstOrDefault(x => x.Active == true);
                    if (section != null)
                    {
                        var question = _questionService.GetAllQuestionsBy(test.Id, obj.sectionId).Where(x => x.OrderNo == obj.question.id).FirstOrDefault(x => x.Active == true);
                        if (question != null)
                        {
                            question.Text = obj.question.text;
                            question.Points = obj.question.points;
                            if (question.QuestionType == QuestionType.Choice)
                            {
                                var options = _answerService.GetAllBy(test.Id, obj.sectionId, obj.question.id).Where(x => x.Active == true);
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
                                question.MixupAnswers = obj.question.mixupOptions;
                            }
                            else if (question.QuestionType == QuestionType.Text)
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
            return Json(new { status = "ERR4" });
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
                public bool mixupOptions { get; set; }
                public int points { get; set; }
                public class Option
                {
                    public int id { get; set; }
                    public string text { get; set; }
                    public bool isCorrect { get; set; }
                }
            }
        }

        public ActionResult SectionUpdate(string testUniqueCode, string name, bool mixupQuestions, int index)
        {
            var test = _testService.GetTestByUniqueNumber(testUniqueCode);
            if (test != null)
            {
               if(_teacherService.IsTeacherOfTest(User.Identity.GetUserId(), test.Id)) { 
                        var section = _sectionService.GetAllSectionsByTestId(test.Id).Where(x => x.OrderNo == index).FirstOrDefault();
                        if (section != null)
                        {
                            section.SectionTitle = name;
                            section.MixupQuestions = mixupQuestions;
                            _sectionService.Update();
                            return Json(new { status = "OK" });
                        }
                    }
                    return Json(new { status = "ERR1" });
                }
            return Json(new { status = "ERR2" });
        }

        public ActionResult ChooseClassesForTest(string testUniqueCode, string chosenClasses)
        {
            var classesCodes = new JavaScriptSerializer().Deserialize<List<string>>(chosenClasses);
            var testRef = _testService.GetTestByUniqueNumber(testUniqueCode);
            var classRefList = new List<Class>();
            if (testRef != null) {
                foreach (var item in classesCodes)
                {
                    var classRef = _classService.GetAll().Where(x => x.UniqueCode == item).FirstOrDefault();
                    if (classRef != null)
                    {
                        classRefList.Add(classRef);
                       
                    }

                }
                var fileName = RandomString(16);
                Session[fileName] = _testGeneration.GenerateFile(testRef, classRefList);
                return Json(new { status = "OK", fName = fileName });
            }
            return Json(new { status = "OK" });
        }
        public ActionResult GetFile(string path)
        {
            var file = Session[path] as FileContentResult;
            if (file == null)
            {
                return new EmptyResult();
            }
            Session[path] = null;
            return file;
        }

        public ActionResult GetTests()
        {
            var teacher = _teacherService.GetTeacherByUserId(User.Identity.GetUserId());
            if (teacher != null)
            {
                var tests = _testService.GetAllTests().Where(x => x.Teacher.Id == teacher.Id);
                List<object> testsResult = new List<object>();
                foreach (var item in tests)
                {
                    if(item.TestTitle == null)
                    {
                        item.TestTitle = "Неозаглавен тест";
                    }
                    testsResult.Add(new { testTitle = item.TestTitle, students = item.Students, averageMark = item.AverageMark, testCode = item.UniqueNumber });
                }
                return Json(new { status = "OK", tests = testsResult }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "ERR1" },  JsonRequestBehavior.AllowGet);
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
                                        questionsList.Add(new { id = question.OrderNo, text = question.Text, points = question.Points, type = (int)question.QuestionType, mixupOptions = question.MixupAnswers, options = optionsList });
                                    } else if (question.QuestionType == QuestionType.Text)
                                    {
                                        questionsList.Add(new { id = question.OrderNo, text = question.Text, points = question.Points, type = (int)question.QuestionType, answerSize = (int)question.QuestionAnswerSize, correctAnswer = question.CorrectAnswer });
                                    }
                                }

                            }
                            sectionsList.Add(new { id = section.OrderNo, text = section.SectionTitle, questions = questionsList});
                                }
                    }
                    return Json(new { status = "OK", sections = sectionsList, testTitle = test.TestTitle });
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
            var classesIEnum = _classService.GetAll().Where(x => x.Teacher.UserId == userId);
            CultureInfo info = new CultureInfo("bg-BG");
            var classes = classesIEnum.OrderBy(x => x.Name, StringComparer.Create(info, false)).ToList();
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

        private static string RandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}