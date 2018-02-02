using MyExams.Models;
using MyExams.Services.Contracts;
using MyExams.TestProcessing.Contracts;
using MyExams.TestProcessing.TestClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.TestProcessing
{
    public class TestGeneration:ITestGeneration
    {   private TPTest tpTest;
        private readonly ITestService _testService;
        private readonly ISectionService _sectionService;
        private readonly IQuestionService _questionService;
        private readonly IAnswerService _answerService;
        private readonly IClassService _classService;
        public Test Test {private get; set; }
        public Student Student { private get; set; }
        public Class Class { private get; set; }
       
        public TestGeneration(ITestService testService, ISectionService sectionService, IQuestionService questionService, IAnswerService answerService, IClassService classService)
        {
            if (_testService == null) _testService = testService;
            if (_sectionService == null) _sectionService = sectionService;
            if (_questionService == null) _questionService = questionService;
            if (_answerService == null) _answerService = answerService;
            if (_classService == null) _classService = classService;
        }
       
        public TPTest GenerateTest()
        {
           
            if (Test == null||Student==null||Class==null) throw new ArgumentNullException();
            var studentClass = _classService.GetClassStudents(Class.UniqueCode).Where(x=>x.Student.Id == Student.Id).FirstOrDefault();
            if (studentClass == null) throw new ArgumentNullException();

            tpTest = new TPTest()
            {
                Title = Test.TestTitle,
                TestId = Test.Id,
                StudentDetails = new TPStudentDetails()
                {
                    ClassName = Class.Name,
                    FullName = Student.FirstName + " " + Student.LastName,
                    StudentId = Student.Id,
                    NoInClass = studentClass.NoInClass
                }
            };
            var sections = _sectionService.GetAllSectionsByTestId(Test.Id);
            var tpSections = new List<TPSection>();
            foreach (var section in sections)
            {
                var tpSection = new TPSection()
                {
                    SectionId = section.Id,
                    Title = section.SectionTitle
                };
                var questions = _questionService.GetAllQuestionsBy(Test.Id, section.OrderNo).ToList();
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
                    var tpQuestion = new TPQuestion()
                    {
                        QuestionId = questions[num].Id,
                        AnswerSize = questions[num].QuestionAnswerSize,
                        maxPoints = questions[num].Points,
                        Title = questions[num].Text,
                        Type = questions[num].QuestionType
                    };
                    if(questions[num].QuestionType == QuestionType.Choice)
                    {
                        var answers = _answerService.GetAllBy(Test.Id, section.OrderNo, questions[num].OrderNo).ToList();
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



            //TODO: save the test
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
