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
        private readonly IGQuestionService _gQuestionService;
        public TestGeneration(ITestService testService, ISectionService sectionService, IQuestionService questionService, IAnswerService answerService, IClassService classService, IGAnswerSheetService gAnswerSheetService, IGQuestionService gQuestionService)
        {
            if (_testService == null) _testService = testService;
            if (_sectionService == null) _sectionService = sectionService;
            if (_questionService == null) _questionService = questionService;
            if (_answerService == null) _answerService = answerService;
            if (_classService == null) _classService = classService;
            if (_gAnswerSheetService == null) _gAnswerSheetService = gAnswerSheetService;
            if (_gQuestionService == null) _gQuestionService = gQuestionService;
        }

        public FileContentResult GenerateFile(Test test, List<Class> classes, Teacher teacher)
        {
            if (test == null && classes == null) throw new ArgumentNullException();
            UpdateIsInUseProperty(test);
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
                    if (section.QuestionsToShow != 0)
                    {
                        questionGenOrder = GenerateNonRepeatingNumbers(0, questions.Count() - 1, section.QuestionsToShow);
                    }
                    else
                    {
                        questionGenOrder = GenerateNonRepeatingNumbers(0, questions.Count() - 1, questions.Count());
                    }
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
                        Type = questions[num].QuestionType,
                         Answers = new List<TPAnswers>()
                    };
                    if (questions[num].QuestionType == QuestionType.Choice)
                    {
                        var answers = _answerService.GetAllBy(test.Id, section.OrderNo, questions[num].OrderNo).ToList();
                        var answersGenOrder = new int[answers.Count];
                        if (questions[num].MixupAnswers)
                        {
                            answersGenOrder = GenerateNonRepeatingNumbers(0, answers.Count - 1, answers.Count());
                        }
                        else
                        {
                            for (int i = 0; i < answers.Count; i++)
                            {
                                answersGenOrder[i] = i;
                            }
                        }
                        
                        foreach (var answerNum in answersGenOrder)
                        {
                            var tpAnswer = new TPAnswers()
                            {
                                AnswerId = answers[answerNum].Id,
                                Text = answers[answerNum].Text
                            };
                            tpQuestion.Answers.Add(tpAnswer);

                        }
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

            _testService.AddNewGTest(gTest);
           
            tpTest.GTestId = gTest.Id;
            SaveGTest(tpTest, gTest);
            
            return tpTest;
        }
        private void SaveGTest(TPTest test, GTest gTest)
        {
            var sections = _sectionService.GetAllSectionsByTestId(test.TestId).OrderBy(x=>x.OrderNo).ToList();
            List<int> questionIds = test.Sections.SelectMany(x => x.Questions.Select(p=>p.QuestionId)).ToList();
            var questions = _questionService.GetAllByIds(questionIds);
            var answers = _answerService.GetAllByQuestionIds(questionIds);
            for(int i = 0; i<sections.Count; i++)
            {
                for (int p = 0; p < test.Sections[i].Questions.Count; p++)
                {
                    var gQuestion = new GQuestion()
                    {
                        Question = questions.Where(x => x.Id == test.Sections[i].Questions[p].QuestionId).FirstOrDefault(),
                        OrderNo = p,
                        Section = sections[i],
                        GTest = gTest
                    };
                    for (int q = 0; q < test.Sections[i].Questions[p].Answers.Count; q++)
                    {
                        var gAnswer = new GAnswer()
                        {
                            Answer = answers.Where(x => x.Id == test.Sections[i].Questions[p].Answers[q].AnswerId).FirstOrDefault(),
                            OrderNo = q, 
                             CheckState = CheckState.NoInfo
                        };
                        
                        gQuestion.GAnswers.Add(gAnswer);
                    }
                    _gQuestionService.AddNewGQuestion(gQuestion);
                    
                }
            }
        }

        private static int[] GenerateNonRepeatingNumbers(int minValue, int maxValue, int count)
        {
            var random = new Random();
            List<int> result = new List<int>();
            var list = new List<int>();
            var counter = 0;
            for (int i = minValue; i < maxValue + 1; i++)
            {
                list.Add(i);
            }
            while (list.Count > maxValue-minValue+1-count)
            {

                var index = random.Next(0, list.Count);
                result.Add(list[index]);
                list.Remove(list[index]);
                counter++;
            }
            return result.ToArray();
        }
        private void UpdateIsInUseProperty(Test test)
        {
            var sections = _sectionService.GetAllSectionsByTestId(test.Id);
            foreach (var section in sections)
            {
                section.IsInUse = true;
                var questions = _questionService.GetAllQuestionsBy(test.Id, section.OrderNo);
                foreach (var question in questions)
                {
                    question.IsInUse = true;
                    var answers = _answerService.GetAllBy(test.Id, section.OrderNo, question.OrderNo);
                    foreach (var answer in answers)
                    {
                        answer.IsInUse = true;
                    }
                }
            }
            _testService.Update();
        }
    }
}
