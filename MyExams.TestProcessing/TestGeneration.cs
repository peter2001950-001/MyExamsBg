using MyExams.Models;
using MyExams.Services.Contracts;
using MyExams.TestProcessing.Contracts;
using MyExams.TestProcessing.TestClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;

namespace MyExams.TestProcessing
{
    public class TestGeneration : ITestGeneration
    {
        private TPTest tpTest;
        private readonly ITestService _testService;
        private readonly ISectionService _sectionService;
        private readonly IQuestionService _questionService;
        private readonly IAnswerService _answerService;
        private readonly IClassService _classService;
        private readonly IGAnswerSheetService _gAnswerSheetService;
        public TestGeneration(ITestService testService, ISectionService sectionService, IQuestionService questionService, IAnswerService answerService, IClassService classService, IGAnswerSheetService gAnswerSheetService)
        {
            if (_testService == null) _testService = testService;
            if (_sectionService == null) _sectionService = sectionService;
            if (_questionService == null) _questionService = questionService;
            if (_answerService == null) _answerService = answerService;
            if (_classService == null) _classService = classService;
            if (_gAnswerSheetService == null) _gAnswerSheetService = gAnswerSheetService;
        }

        public FileContentResult GenerateFile(Test test, List<Class> classes, Teacher teacher)
        {
            if (test == null && classes == null) throw new ArgumentNullException();
            var allTPTest = new List<TPTest>();
            foreach (var classItem in classes)
            {
                var studentsInClass = _classService.GetClassStudents(classItem.UniqueCode);
                foreach (var studentClass in studentsInClass)
                {
                    var tpTest = GenerateTest(test, studentClass.Student, classItem, teacher);
                    if (tpTest != null)
                    {
                        allTPTest.Add(tpTest);
                    }
                }
            }

            PdfBuilder builder = new PdfBuilder(_testService, _gAnswerSheetService);
            return builder.TPTestToPdf(allTPTest);

            //export to PDF and return result
        }

        private TPTest GenerateTest(Test test, Student student, Class classObj, Teacher teacher)
        {

            if (test == null || student == null || classObj == null && teacher == null) throw new ArgumentNullException();
            var studentClass = _classService.GetClassStudents(classObj.UniqueCode).Where(x => x.Student.Id == student.Id).FirstOrDefault();
            if (studentClass == null) throw new ArgumentNullException();
            int points = 0;

            tpTest = new TPTest()
            {
                Title = test.TestTitle,
                TestId = test.Id,
                StudentDetails = new TPStudentDetails()
                {
                    ClassName = classObj.Name,
                    FullName = student.FirstName + " " + student.LastName,
                    StudentId = student.Id,
                    NoInClass = studentClass.NoInClass
                }
            };
            var sections = _sectionService.GetAllSectionsByTestId(test.Id);
            var tpSections = new List<TPSection>();
            foreach (var section in sections)
            {
                var tpSection = new TPSection()
                {
                    SectionId = section.Id,
                    Title = section.SectionTitle
                };
                var questions = _questionService.GetAllQuestionsBy(test.Id, section.OrderNo).ToList();
                var questionGenOrder = new int[questions.Count];
                if (section.MixupQuestions)
                {
                    questionGenOrder = GenerateNonRepeatingNumbers(0, questions.Count() - 1);
                }
                else
                {
                    for (int i = 0; i < questions.Count; i++)
                    {
                        questionGenOrder[i] = i;
                    }
                }
                var tpQuestions = new List<TPQuestion>();
                foreach (var num in questionGenOrder)
                {
                    points += questions[num].Points;
                    var tpQuestion = new TPQuestion()
                    {
                        QuestionId = questions[num].Id,
                        AnswerSize = questions[num].QuestionAnswerSize,
                        maxPoints = questions[num].Points,
                        Title = questions[num].Text,
                        Type = questions[num].QuestionType
                    };
                    if (questions[num].QuestionType == QuestionType.Choice)
                    {
                        var answers = _answerService.GetAllBy(test.Id, section.OrderNo, questions[num].OrderNo).ToList();
                        var answersGenOrder = new int[answers.Count];
                        if (questions[num].MixupAnswers)
                        {
                            answersGenOrder = GenerateNonRepeatingNumbers(0, answers.Count - 1);
                        }
                        else
                        {
                            for (int i = 0; i < answers.Count; i++)
                            {
                                answersGenOrder[i] = i;
                            }
                        }

                        var tpAnswers = new List<TPAnswers>();
                        foreach (var answerNum in answersGenOrder)
                        {
                            var tpAnswer = new TPAnswers()
                            {
                                AnswerId = answers[answerNum].Id,
                                Text = answers[answerNum].Text
                            };
                            tpAnswers.Add(tpAnswer);

                        }
                        tpQuestion.Answers = tpAnswers;

                    }
                    tpQuestions.Add(tpQuestion);
                }
                tpSection.Questions = tpQuestions;
                tpSections.Add(tpSection);
            };
            tpTest.Sections = tpSections;
            tpTest.MaxPoints = points;


            var gTest = new GTest()
            {
                Test = test,
                MaxPoints = points,
                Student = student,
                Teacher = teacher,
                 Class = classObj
            };
            string xml;
            using (var sw = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(sw))
                {
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteStartElement("qs"); // short for questions

                    foreach (var section in tpTest.Sections)
                    {
                        for (int i = 0; i < section.Questions.Count; i++)
                        {
                            xmlWriter.WriteStartElement("q"); // short for question
                            xmlWriter.WriteAttributeString("id", section.Questions[i].QuestionId.ToString());

                            if (section.Questions[i].Type == QuestionType.Choice)
                            {
                                for (int p = 0; p < section.Questions[i].Answers.Count; p++)
                                {
                                    xmlWriter.WriteStartElement("a"); // short for answer
                                    xmlWriter.WriteAttributeString("id", section.Questions[i].Answers[p].AnswerId.ToString());
                                    xmlWriter.WriteEndElement();
                                }
                            }
                            xmlWriter.WriteEndElement();
                        }
                    }
                    xmlWriter.WriteEndDocument();
                }
                xml = sw.ToString();
            }
            gTest.Xml = xml;
            _testService.AddNewGTest(gTest);
           
            tpTest.GTestId = gTest.Id;
            return tpTest;
        }

        private static int[] GenerateNonRepeatingNumbers(int minValue, int maxValue)
        {
            var random = new Random();
            int[] result = new int[maxValue - minValue + 1];
            var list = new List<int>();
            var counter = 0;
            for (int i = minValue; i < maxValue + 1; i++)
            {
                list.Add(i);
            }
            while (list.Count > 0)
            {

                var index = random.Next(0, list.Count);
                result[counter] = list[index];
                list.Remove(list[index]);
                counter++;
            }
            return result;
        }
    }
}
