using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNet.Identity;
using MyExams.Models;
using MyExams.Services.Contracts;
using MyExams.TestProcessing.Contracts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;

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
        private readonly IGAnswerSheetService _gAnswerSheetService;
        private readonly IFileDirectoryService _fileDirectoryService;
        private readonly IUploadSessionService _uploadSessionService;
        private readonly ITestCheckProcess _testCheckProcess;
        private readonly IMonitoringApi _monitoringApi;
        private readonly IGQuestionService _gQuestionService;
        private readonly IGAnswerService _gAnswerService;

        public int GTestId { get; private set; }

        public TeacherController(IClassService classService, IStudentService studentService, ITestService testService, ITeacherService teacherService, ISectionService sectionService, IQuestionService questionService, IAnswerService answerService, ITestGeneration testGeneration, IGAnswerSheetService gAnswerSheetService, ITestCheckProcess testCheckProcess, IFileDirectoryService fileDirectoryService, IUploadSessionService uploadSessionService, IGQuestionService gQuestionService, IGAnswerService gAnswerService)
        {
            _classService = classService;
            _studentService = studentService;
            _testService = testService;
            _teacherService = teacherService;
            _sectionService = sectionService;
            _questionService = questionService;
            _answerService = answerService;
            _testGeneration = testGeneration;
            _gAnswerSheetService = gAnswerSheetService;
            _fileDirectoryService = fileDirectoryService;
            _uploadSessionService = uploadSessionService;
            _testCheckProcess = testCheckProcess;
            _monitoringApi = JobStorage.Current.GetMonitoringApi();
            _gQuestionService = gQuestionService;
            _gAnswerService = gAnswerService;
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

            return Json(new { status = "OK", id = test.UniqueNumber });
        }

        public ActionResult TestDesign(string id)
        {
            var test = _testService.GetTestByUniqueNumber(id);
            if (_teacherService.IsTeacherOfTest(User.Identity.GetUserId(), test.Id))
            {
                ViewBag.id = id;
                ViewBag.Title = test.TestTitle;
                return View();
            }
            return RedirectToAction("tests");
        }

        [HttpGet]
        public ActionResult UploadFiles()
        {
            ViewBag.Title = "Качване на файлове";
            return View();
        }
        public ActionResult GTest(int id)
        {
            ViewBag.id = id;
            var gTest = _testService.GetAllGTestIncludeAll().Where(x => x.Id == id).FirstOrDefault();
            if (gTest != null)
            {
                ViewBag.Title = gTest.Test.TestTitle;
                return View();
            }
            return RedirectToAction("Index");
        }
        public JsonResult GetGTest(int id)
        {
            var userId = User.Identity.GetUserId();
            if (_teacherService.IsTeacherOfGTest(userId, id))
            {
                var gTest = _testService.GetAllGTestIncludeAll().Where(x => x.Id == id).FirstOrDefault();
                if (gTest != null)
                {
                    var alphabet = "АБВГДЕЖЗИЙКЛМНОПРСТ";
                    var gQuestions = _gQuestionService.GetAllBy(gTest.Id).OrderBy(x => x.OrderNo).ToList();
                    List<GWrittenQuestion> writtenQuestions = _gAnswerSheetService.GetGWrittenQuestionsBy(gTest.Id).ToList();

                    var sections = new List<Section>();
                    foreach (var item in gQuestions)
                    {
                        if (!sections.Any(x => x.Id == item.Section.Id))
                        {
                            sections.Add(item.Section);
                        }
                    }

                    var questionCount = 0;
                    List<object> sectionObjects = new List<object>();
                    foreach (var section in sections)
                    {
                        List<object> questionObjects = new List<object>();
                        var questionsOfSection = gQuestions.Where(x => x.Section.Id == section.Id).ToList();
                        foreach (var item in questionsOfSection)
                        {
                            questionCount++;
                            var answerCount = -1;
                            if (item.GAnswers.Count > 0)
                            {
                                List<object> answerObjects = new List<object>();
                                foreach (var answer in item.GAnswers.OrderBy(x => x.OrderNo))
                                {
                                    answerCount++;
                                    var answerText = alphabet[answerCount] + ") " + answer.Answer.Text;
                                    if (answer.CheckState == CheckState.NotChecked && answer.Answer.IsCorrect)
                                    {
                                        answerObjects.Add(new { text = answerText, color = "#4dbd74" }); // green color - the answer is correct but not marked
                                    }
                                    else if (answer.CheckState == CheckState.Checked)
                                    {
                                        answerObjects.Add(new { text = answerText, color = "#f43f3f" }); // red color - the answer is not correct but marked
                                    }
                                    else if (answer.CheckState == CheckState.Correct)
                                    {
                                        answerObjects.Add(new { text = answerText, color = "#4dbd74" });
                                    }
                                    else if (answer.CheckState == CheckState.NotChecked)
                                    {
                                        answerObjects.Add(new { text = answerText, color = "#000000" });
                                    }
                                }
                                var points = item.ReceivedPoints;
                                var color = "";
                                if (points > 0)
                                {
                                    color = "#4dbd74";
                                }
                                else
                                {
                                    color = "#f43f3f";
                                }
                                questionObjects.Add(new { text = questionCount + ". " + item.Question.Text, answers = answerObjects, points = points, pointsColor = color, type = 0 });

                            }
                            else
                            {
                                var result = questionsOfSection.IndexOf(questionsOfSection.First(x => x.Id == item.Id));
                                var writtenQuestion = writtenQuestions.Where(x => x.GQuestionId == result).FirstOrDefault();
                                if (writtenQuestion != null)
                                {

                                    var image = "";
                                    try
                                    {
                                        var srcImage = Image.FromFile(writtenQuestion.FileName);
                                        using (var stream = new MemoryStream())
                                        {
                                            srcImage.Save(stream, ImageFormat.Jpeg);
                                            image = "data:image/png;base64," + Convert.ToBase64String(stream.ToArray());
                                        }
                                    }
                                    catch (Exception)
                                    {


                                    }

                                    var points = item.ReceivedPoints;
                                    var color = "";
                                    if (points > 0)
                                    {
                                        color = "#4dbd74";
                                    }
                                    else
                                    {
                                        color = "#f43f3f";
                                    }

                                    questionObjects.Add(new { text = questionCount + ". " + item.Question.Text, correctAnswer = item.Question.CorrectAnswer, points = points, image = image, pointsColor = color, type = 1 });

                                }
                            }

                        }
                        sectionObjects.Add(new { text = section.SectionTitle, questions = questionObjects });
                    }
                    var studentClass = _classService.GetAllClassStudents().Where(x => x.Student?.Id == gTest.Student.Id && x.Class?.Id == gTest.Class.Id).FirstOrDefault();

                    return Json(new { test = new { title = gTest.Test.TestTitle, studentName = gTest.Student.FirstName + " " + gTest.Student.LastName, noInClass = studentClass.NoInClass, sections = sectionObjects, receivedPoints = gTest.ReceivedPoints, totalPoints = gTest.MaxPoints, percentage = Math.Round(((double)gTest.ReceivedPoints * 100) / (double)gTest.MaxPoints, 2), mark = Math.Round(((double)gTest.ReceivedPoints * 6) / (double)gTest.MaxPoints, 2) }, status = "OK" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "ERR1" });
            }
            return Json(new { status = "ERR2" });
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
        public ActionResult ElementUpdate(string type, string action, int index, string testUniqueCode, int sectionId = 0, int questionId = 0, int questionType = 0)
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
                            var newQuestion = _questionService.QuestionHasChanged(obj, question);

                            if (question.QuestionType == QuestionType.Choice)
                            {
                                var options = _answerService.GetAllBy(test.Id, obj.sectionId, obj.question.id).ToList();
                                foreach (var item in options)
                                {
                                    var option = obj.question.options.Where(x => x.id == item.OrderNo).FirstOrDefault();
                                    _answerService.AnswerHasChanged(option, item);
                                }
                            }
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


        public ActionResult SectionUpdate(string testUniqueCode, string name, bool mixupQuestions, int index, int questionsToShow)
        {
            var test = _testService.GetTestByUniqueNumber(testUniqueCode);

            if (test != null)
            {
                if (_teacherService.IsTeacherOfTest(User.Identity.GetUserId(), test.Id))
                {
                    var section = _sectionService.GetAllSectionsByTestId(test.Id).Where(x => x.OrderNo == index).FirstOrDefault();
                    if (section != null)
                    {
                        var questions = _questionService.GetAllQuestionsBy(test.Id, section.OrderNo);
                        if (questionsToShow <= questions.Count())
                        {
                            section.SectionTitle = name;
                            section.MixupQuestions = mixupQuestions;
                            section.QuestionsToShow = questionsToShow;
                            _sectionService.Update();
                            return Json(new { status = "OK" });
                        }
                        return Json(new { status = "ERR3" });
                    }
                }
                return Json(new { status = "ERR1" });
            }
            return Json(new { status = "ERR2" });
        }

        public ActionResult ChooseClassesForTest(string testUniqueCode, string chosenClasses)
        {
            var userId = User.Identity.GetUserId();
            var teacher = _teacherService.GetTeacherByUserId(userId);
            if (teacher != null)
            {
                var classesCodes = new JavaScriptSerializer().Deserialize<List<string>>(chosenClasses);
                var testRef = _testService.GetTestByUniqueNumber(testUniqueCode); ;
                var classRefList = new List<Class>();
                if (testRef != null)
                {
                    foreach (var item in classesCodes)
                    {
                        var classRef = _classService.GetAll().Where(x => x.UniqueCode == item).FirstOrDefault();
                        classRef.RecentUsage = DateTime.Now;
                        if (classRef != null)
                        {
                            classRefList.Add(classRef);

                        }
                        _testService.Update();

                    }
                    var fileName = RandomString(16, true);
                    Session[fileName] = _testGeneration.GenerateFile(testRef, classRefList, teacher);
                    return Json(new { status = "OK", fName = fileName });
                }
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
                var testsResult = _testService.GetTestObjects(teacher.UserId);

                return Json(new { status = "OK", tests = testsResult }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "ERR1" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTest(string testUniqueCode)
        {
            var test = _testService.GetTestByUniqueNumber(testUniqueCode);
            var teacher = _teacherService.GetTeacherByUserId(User.Identity.GetUserId());
            if (test != null)
            {

                if (_teacherService.IsTeacherOfTest(User.Identity.GetUserId(), test.Id))
                {

                    var testObj = _testService.GetTestByUniqueNumber(testUniqueCode);
                    if (testObj != null)
                    {
                        var sections = _sectionService.GetAllSectionsByTestId(testObj.Id).OrderBy(x => x.OrderNo).ToList();
                        List<object> sectionsList = new List<object>();

                        foreach (var section in sections)
                        {
                            if (section.Active)
                            {
                                var questionsList = new List<object>();

                                var questions = _questionService.GetAllQuestionsBy(test.Id, section.OrderNo).OrderBy(x => x.OrderNo).ToList();
                                foreach (var question in questions)
                                {
                                    if (question.Active)
                                    {
                                        var optionsList = new List<object>();
                                        if (question.QuestionType == QuestionType.Choice)
                                        {
                                            var options = _answerService.GetAllBy(test.Id, section.OrderNo, question.OrderNo).OrderBy(x => x.OrderNo).ToList();
                                            foreach (var option in options)
                                            {
                                                if (option.Active)
                                                {
                                                    optionsList.Add(new { id = option.OrderNo, text = option.Text, isCorrect = option.IsCorrect });
                                                }

                                            }
                                            questionsList.Add(new { id = question.OrderNo, text = question.Text, points = question.Points, type = (int)question.QuestionType, mixupOptions = question.MixupAnswers, options = optionsList });
                                        }
                                        else if (question.QuestionType == QuestionType.Text)
                                        {
                                            questionsList.Add(new { id = question.OrderNo, text = question.Text, points = question.Points, type = (int)question.QuestionType, answerSize = (int)question.QuestionAnswerSize, correctAnswer = question.CorrectAnswer });
                                        }
                                    }

                                }
                                var image = "";
                                if (section.ImageFileName != null)
                                {
                                    try
                                    {
                                        var srcImage = Image.FromFile(Path.Combine(Server.MapPath("~/App_Data"), section.ImageFileName));
                                        using (var MemoryStream = new MemoryStream())
                                        {
                                            srcImage.Save(MemoryStream, ImageFormat.Jpeg);

                                            image = "data:image/png;base64," + Convert.ToBase64String(MemoryStream.ToArray());
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }

                                }
                                sectionsList.Add(new { id = section.OrderNo, text = section.SectionTitle, questionsToShow = section.QuestionsToShow, questions = questionsList, mixupQuestions = section.MixupQuestions, image = image });

                            }
                        }
                        return Json(new { status = "OK", sections = sectionsList, testTitle = test.TestTitle });
                    }
                    return Json(new { status = "ERR1" });
                }
                return Json(new { status = "ERR2" });
            }
            return Json(new { status = "ERR3" });
        }

        public JsonResult GetTestStat(string testUniqueCode, bool all, string classesIds = "")
        {
            var teacher = _teacherService.GetTeacherByUserId(User.Identity.GetUserId());
            if (teacher != null)
            {
                var test = _testService.GetTestByUniqueNumber(testUniqueCode);
                if (_teacherService.IsTeacherOfTest(User.Identity.GetUserId(), test.Id))
                {
                    if (all)
                    {
                        var gTests = _testService.GetAllGTestIncludeAll().Where(x => x.Teacher.Id == teacher.Id).Where(x => x.IsDone).Where(x => x.Test.Id == test.Id).ToList();
                        var classes = gTests.Select(x => x.Class).Distinct();

                        var marks = gTests.Select(x => Math.Round((double)x.ReceivedPoints / (double)x.MaxPoints * 6, 2) < 2 ? 2 : Math.Round((double)x.ReceivedPoints / (double)x.MaxPoints * 6, 2));
                        if (marks.Count() != 0)
                        {
                            var gQuestions = _gQuestionService.GetAllBy(gTests.Select(x => x.Id).ToList());

                            var questionsObj = _testService.GetAnalysisBy(gTests.Select(x => x.Id).ToList(), test);
                            var classesObj = classes.Select(x => new { text = x.Name, id = x.Id, isChecked = true }).ToList();

                            return Json(new { status = "OK", averageMark = Math.Round(marks.DefaultIfEmpty(0).Average(), 2), twos = marks.Count(x => x >= 2 && x < 3), threes = marks.Count(x => x >= 3 && x < 3.5), fours = marks.Count(x => x >= 3.5 && x < 4.5), fives = marks.Count(x => x >= 4.5 && x < 5.5), sixs = marks.Count(x => x >= 5.5), questions = questionsObj, classes = classesObj }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    return Json(new { status = "NONE" });
                }
            }
            return Json(new { status = "ERR" });
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

        public JsonResult SyncIndex()
        {
            var userId = User.Identity.GetUserId();
            var teacher = _teacherService.GetTeacherByUserId(userId);
            if (teacher != null)
            {
                bool isQuestionsToBeChecked = false;
                var count = 0;
                var classesResultObj = _classService.GetClassObjects(userId, x => x.RecentUsage, Services.OrderByMethod.Descending).Take(3);
                var testsResultObj = _testService.GetTestObjects(userId, x => x.RecentUsage, Services.OrderByMethod.Descending).Take(3);

                if (_gAnswerSheetService.GetAllGQuestionToBeCheckedBy(teacher.Id).Count() > 0)
                {
                    isQuestionsToBeChecked = true;
                    count = _gAnswerSheetService.GetAllGQuestionToBeCheckedBy(teacher.Id).Count();
                }

                return Json(new { status = "OK", classes = classesResultObj, tests = testsResultObj, isQuestionsToBeChecked = isQuestionsToBeChecked, count = count }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { status = "ERR" }, JsonRequestBehavior.AllowGet);
        }
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewClass(NewClassViewModel model)
        {
            var userId = User.Identity.GetUserId();
            var addedClass = _classService.CreateNewClass(userId, model.Name, model.Subject);
            addedClass.RecentUsage = DateTime.Now;
            _testService.Update();
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
            var classesObj = _classService.GetClassObjects(userId, x => x.Name, Services.OrderByMethod.Ascending);

            return Json(new { classes = classesObj, status = "OK" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStudents(string uniqueCode)
        {
            var userId = User.Identity.GetUserId();
            if (_classService.IsTeacherOfClass(userId, uniqueCode))
            {
                var students = _classService.GetClassStudents(uniqueCode).OrderBy(x => x.NoInClass).ToList();
                var classObj = _classService.GetAll().First(x => x.UniqueCode == uniqueCode);
                object[] studentsOutput = new object[students.Count];
                var allGtests = _testService.GetGTestBy(classObj.Id).ToList();
                var tests = new List<Test>();
                foreach (var item in allGtests)
                {
                    if (!tests.Any(x => x.Id == item.Test.Id))
                    {
                        tests.Add(item.Test);
                    }
                }
                for (int i = 0; i < students.Count; i++)
                {
                    var marks = new List<object>();


                    foreach (var test in tests)
                    {
                        var studentTries = allGtests.Where(x => x.Test.Id == test.Id && x.Student.Id == students[i].Student.Id).ToList();
                        if (studentTries.Count == 0)
                        {
                            marks.Add(new { });
                        }
                        else
                        {
                            marks.Add(studentTries.Select(x => new { rp = x.ReceivedPoints, tp = x.MaxPoints, percentage = Math.Round(((double)x.ReceivedPoints * 100) / (double)x.MaxPoints, 2), mark = Math.Round(((double)x.ReceivedPoints * 6) / (double)x.MaxPoints, 2) < 2 ? 2.00 : Math.Round(((double)x.ReceivedPoints * 6) / (double)x.MaxPoints, 2), id = x.Id }));
                        }
                    }
                    studentsOutput[i] = new { firstName = students[i].Student.FirstName, lastName = students[i].Student.LastName, noInClass = students[i].NoInClass, marks = marks };

                }
                var testTitles = tests.Select(x => new { title = x.TestTitle });
                return Json(new { students = studentsOutput, tests = testTitles, status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "ERR" }, JsonRequestBehavior.AllowGet);

        }
        [ValidateAntiForgeryToken]
        public JsonResult AddStudent(string firstName, string lastName, string no, string classCode)
        {
            if (_classService.IsTeacherOfClass(User.Identity.GetUserId(), classCode))
            {
                var classObj = _classService.GetAll().Where(x => x.UniqueCode == classCode).First();
                int numberInClass = 0;
                if (int.TryParse(no, out numberInClass))
                {
                    if (!_classService.GetClassStudents(classCode).Any(x => x.NoInClass == numberInClass))
                    {
                        Student student = new Student()
                        {
                            FirstName = firstName,
                            LastName = lastName
                        };
                        _studentService.AddStudent(student);
                        _classService.AddStudentToClass(student, classCode, numberInClass);
                        return Json(new { status = "OK" });
                    }
                    return Json(new { status = "ERR1" });
                }
                return Json(new { status = "ERR2" });
            }
            return Json(new { status = "ERR3" });
        }

        [HttpPost]
        public JsonResult UploadFilesProcess()
        {
            var teacher = _teacherService.GetTeacherByUserId(User.Identity.GetUserId());

            var uploadSession = new UploadSession()
            {
                Teacher = teacher,
                IsActive = true

            };
            _uploadSessionService.AddUploadSession(uploadSession);
            foreach (string file in Request.Files)
            {
                var fileContent = Request.Files[file];
                if (fileContent != null && fileContent.ContentLength > 0)
                {
                    // get a stream
                    var getExtension = Path.GetExtension(fileContent.FileName);
                    var newFileName = RandomString(16, false) + getExtension;
                    var stream = fileContent.InputStream;
                    var path = Path.Combine(Server.MapPath("~/App_Data"), newFileName);
                    var serverPath = Path.Combine(Server.MapPath("~/App_Data"));
                    fileContent.SaveAs(Path.Combine(Server.MapPath("~/App_Data"), newFileName));
                    var fileDirectory = new FileDirectory()
                    {
                        FileName = path,
                        OriginalFileName = fileContent.FileName
                    };
                    _fileDirectoryService.AddFileDirectory(fileDirectory);
                    var uploadSessionFileDirectory = new UploadSessionFileDirectory()
                    {
                        FileDirectory = fileDirectory,
                        UploadedFileStatus = UploadedFileStatus.NotChecked,
                        UploadSession = uploadSession
                    };
                    _uploadSessionService.AddUploadSessionFileDirectory(uploadSessionFileDirectory);
                    uploadSessionFileDirectory.JobId = BackgroundJob.Enqueue(() => TestCheck(fileDirectory, serverPath));
                    _testService.Update();
                }
                uploadSession.TotalUploaded++;
                _testService.Update();

            }
            if (!_uploadSessionService.GetAll().Any(x => x.IsActive == true && x.Id != uploadSession.Id))
            {
                BackgroundJob.Enqueue(() => UploadSessionsTracer());
            }

            return Json(new { status = "OK", sessionId = uploadSession.SessionIdentifier });
        }
        [AutomaticRetry(Attempts = 0)]
        public void TestCheck(FileDirectory fileDirectory, string serverFileName)
        {
            Bitmap bitmap = (Bitmap)Image.FromFile(fileDirectory.FileName);
            _testCheckProcess.SetBitmap(bitmap);
            _testCheckProcess.SetSaveFileName(serverFileName);
            _testCheckProcess.SetBitmapFileDirectory(fileDirectory);

            if (_testCheckProcess.StartChecking() == UploadedFileStatus.AlreadyChecked)
            {
                bitmap.Dispose();

            }
        }

        public void UploadSessionsTracer()
        {
            while (true)
            {
                _uploadSessionService.ClearCache();
                Thread.Sleep(1000);
                var activeSessions = _uploadSessionService.GetAll().Where(x => x.IsActive == true).ToList();
                if (activeSessions.Count() > 0)
                {
                    foreach (var uploadSession in activeSessions)
                    {
                        var uploadSessionFiles = _uploadSessionService.GetUploadSessionFileDirectoriesBy(uploadSession).ToList();
                        int totalFinished = 0;
                        foreach (var item in uploadSessionFiles)
                        {
                            var state = _monitoringApi.JobDetails(item.JobId);
                            if (state.History.First().StateName == "Succeeded" || state.History.First().StateName == "Failed")
                            {
                                totalFinished++;
                            }
                        }
                        uploadSession.TotalFinished = totalFinished;
                        if (totalFinished == uploadSession.TotalUploaded)
                        {
                            uploadSession.IsActive = false;
                            var enqueueTests = new List<int>();
                            foreach (var file in uploadSessionFiles)
                            {
                                if (file.AnswerSheet != null)
                                {
                                    var answerSheet = _gAnswerSheetService.GetAllGAnswerSheet().Where(x => x.Id == file.AnswerSheet.Id).First();
                                    if (answerSheet.AnswerSheetStatus == AnswerSheetStatus.Checked)
                                    {
                                        var id = _testService.GetGTestBy(file.AnswerSheet).Id;
                                        if (!enqueueTests.Any(x => x == id))
                                        {
                                            BackgroundJob.Enqueue(() => TestResultUpdate(id));
                                            enqueueTests.Add(id);
                                        }
                                    }
                                }
                            }
                        }
                        _testService.Update();

                    }

                }
                else
                {
                    break;
                }
            }
        }
        public void TestResultUpdate(int GtestId)
        {
            _gAnswerSheetService.ClearAnswerSheetCache();
            _testService.ClearGTestCache();

            var answerSheets = _gAnswerSheetService.GetGAnswerSheetsBy(GtestId).ToList();
            if (answerSheets != null)
            {
                if (answerSheets.All(x => x.AnswerSheetStatus == AnswerSheetStatus.Checked))
                {
                    var gTest = _testService.GetAllGTestIncludeAll().FirstOrDefault(x => x.Id == GtestId);

                    if (gTest != null)
                    {
                        var questions = _gQuestionService.GetAllBy(gTest.Id);

                        int totalPoints = questions.Sum(x => x.ReceivedPoints);

                        gTest.ReceivedPoints = totalPoints;
                        gTest.IsDone = true;
                        _testService.Update();


                        var marks = _testService.GetAllGTestIncludeAll().Where(x => x.Class?.Id == gTest.Class.Id && x.IsDone).Select(x => Math.Round(((double)x.ReceivedPoints / (double)x.MaxPoints) * 6, 2) < 2 ? 2 : Math.Round(((double)x.ReceivedPoints / (double)x.MaxPoints) * 6, 2));
                        var classObj = _classService.GetAll().Where(x => x.Id == gTest.Class.Id).First();
                        classObj.AverageMark = Math.Round(marks.Average(), 2);

                        _testService.Update();

                        var testMarks = _testService.GetAllGTestIncludeAll().Where(x => x.Test.Id == gTest.Test.Id && x.IsDone).Select(x => Math.Round(((double)x.ReceivedPoints / (double)x.MaxPoints) * 6, 2) < 2 ? 2 : Math.Round(((double)x.ReceivedPoints / (double)x.MaxPoints) * 6, 2));
                        var testObj = _testService.GetAllTests().Where(x => x.Id == gTest.Test.Id).First();
                        testObj.AverageMark = Math.Round(testMarks.Average(), 2);
                        _testService.Update();
                    }
                }
            }
        }
        public JsonResult UploadSessionPull(string sessionIdentifier)
        {
            _uploadSessionService.ClearCache();
            var teacher = _teacherService.GetTeacherByUserId(User.Identity.GetUserId());
            var session = _uploadSessionService.GetAll().Where(x => x.SessionIdentifier == sessionIdentifier).FirstOrDefault();
            if (session != null)
            {
                if (teacher != null)
                {
                    if (session.Teacher.Id == teacher.Id)
                    {
                        if (!session.IsDone)
                        {
                            double percentage = Math.Round((double)(session.TotalFinished / session.TotalUploaded) * 100, 0);
                            return Json(new { status = "OK", percentage = percentage + "%" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { status = "OK", percentage = "100%" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                return Json(new { status = "ERR1" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "ERR2" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UploadSessionNotification()
        {
            var teacher = _teacherService.GetTeacherByUserId(User.Identity.GetUserId());
            if (teacher != null)
            {
                var session = _uploadSessionService.GetAll().Where(x => x.IsNotified == false && x.Teacher?.Id == teacher.Id && x.IsDone).FirstOrDefault();
                if (session != null)
                {
                    List<object> fileDirectoryResult = new List<object>();
                    var uploadSessionFileDirectories = _uploadSessionService.GetUploadSessionFileDirectoriesBy(session);
                    foreach (var item in uploadSessionFileDirectories)
                    {
                        fileDirectoryResult.Add(new { fileStatus = item.UploadedFileStatus, fileName = item.FileDirectory.OriginalFileName, id = item.FileDirectory.Id });

                    }
                    session.IsNotified = true;
                    _testService.Update();
                    return Json(new { status = "HAS", files = fileDirectoryResult }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "NO" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "ERR" }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetWrittenQuestion()
        {
            var teacher = _teacherService.GetTeacherByUserId(User.Identity.GetUserId());
            if (teacher != null)
            {
                _gAnswerSheetService.ClearGQuestionsToBeCheckedCache();
                var questionToBeChecked = _gAnswerSheetService.GetAllGQuestionToBeCheckedBy(teacher.Id).LastOrDefault();
                if (questionToBeChecked != null)
                {
                    if (questionToBeChecked.GWrittenQuestion != null)
                    {

                        var gTest = _testService.GetAllGTests().Where(x => x.Id == questionToBeChecked.GWrittenQuestion.GTest.Id).First();
                        var gQuestions = _gQuestionService.GetAllBy(gTest.Id).OrderBy(x => x.OrderNo).ToList();


                        var gQuestion = gQuestions[questionToBeChecked.GWrittenQuestion.GQuestionId];


                        if (questionToBeChecked.GWrittenQuestion.QuestionType == QuestionType.Text)
                        {
                            var srcImage = Image.FromFile(questionToBeChecked.GWrittenQuestion.FileName);
                            using (var stream = new MemoryStream())
                            {
                                srcImage.Save(stream, ImageFormat.Jpeg);

                                return Json(new { status = "OK", question = new { type = 1, text = gQuestion.Question.Text, options = gQuestion.Question.Points, correctAnswer = gQuestion.Question.CorrectAnswer, src = "data:image/png;base64," + Convert.ToBase64String(stream.ToArray()), id = questionToBeChecked.Id } }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            var answersCount = gQuestion.GAnswers.Count;
                            var srcImage = Image.FromFile(questionToBeChecked.GWrittenQuestion.FileName);
                            using (var stream = new MemoryStream())
                            {
                                srcImage.Save(stream, ImageFormat.Jpeg);

                                return Json(new { status = "OK", question = new { type = 0, text = gQuestion.Question.Text, options = answersCount, src = "data:image/png;base64," + Convert.ToBase64String(stream.ToArray()), id = questionToBeChecked.Id } }, JsonRequestBehavior.AllowGet);
                            }
                        }

                    }
                    return Json(new { status = "ERR2" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "ERR3" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "ERR4" }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GivePoints(int questionId, int option)
        {
            var teacher = _teacherService.GetTeacherByUserId(User.Identity.GetUserId());
            if (teacher != null)
            {
                if (_gAnswerSheetService.GetAllGQuestionToBeCheckedBy(teacher.Id).Any(x => x.Id == questionId))
                {
                    var question = _gAnswerSheetService.GetAllGQuestionToBeCheckedBy(teacher.Id).Where(x => x.Id == questionId).FirstOrDefault();
                    var answerSheets = _gAnswerSheetService.GetGAnswerSheetsBy(question.GWrittenQuestion.GTest.Id).ToList();
                    var answerSheetNeeded = _gAnswerSheetService.GetGAnswerSheetBy(teacher.Id, questionId);
                    if (answerSheetNeeded != null)
                    {

                        var gQuestions = _gQuestionService.GetAllBy(question.GWrittenQuestion.GTest.Id).OrderBy(x => x.OrderNo).ToList();
                        if (question.GWrittenQuestion.QuestionType == QuestionType.Text)
                        {
                            gQuestions[question.GWrittenQuestion.GQuestionId].ReceivedPoints = option;
                        }
                        else
                        {

                            var answers = gQuestions[question.GWrittenQuestion.GQuestionId].GAnswers.OrderBy(x => x.OrderNo).ToList();
                            bool isCorrect = false;
                            for (int i = 0; i < answers.Count; i++)
                            {
                                if (answers[i].Answer.IsCorrect && i == option - 1)
                                {
                                    answers[i].CheckState = CheckState.Correct; // correct
                                    isCorrect = true;
                                }
                                else if (!answers[i].Answer.IsCorrect && i == option - 1)
                                {
                                    answers[i].CheckState = CheckState.Checked; // checked but not correct
                                }
                                else if (i != option - 1)
                                {
                                    answers[i].CheckState = CheckState.NotChecked;
                                }

                            }
                            _testService.Update();
                            if (isCorrect)
                            {
                                gQuestions[question.GWrittenQuestion.GQuestionId].ReceivedPoints = gQuestions[question.GWrittenQuestion.GQuestionId].Question.Points;
                            }
                            else
                            {
                                gQuestions[question.GWrittenQuestion.GQuestionId].ReceivedPoints = 0;

                            }

                        }

                        question.GWrittenQuestion.IsChecked = true;
                        _testService.Update();

                        if (_gAnswerSheetService.GetGWrittenQuestionsBy(question.GWrittenQuestion.GTest.Id).Count(x => x.IsChecked == false) == 0)
                        {
                            foreach (var item in answerSheets)
                            {
                                item.AnswerSheetStatus = AnswerSheetStatus.Checked;
                            }
                            _testService.Update();
                            BackgroundJob.Enqueue(() => TestResultUpdate(question.GWrittenQuestion.GTest.Id));
                        }
                        _gAnswerSheetService.RemoveGQuestionToBeChecked(question);
                        return Json(new { status = "OK" });
                    }
                    return Json(new { status = "ERR1" });
                }
                return Json(new { status = "ERR2" });
            }
            return Json(new { status = "ERR3" });
        }
        private static string RandomString(int length, bool OnlyUppperCase)
        {
            var random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            if (OnlyUppperCase)
            {
                chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            }

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}