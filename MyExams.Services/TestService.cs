using MyExams.Database.Contracts;
using MyExams.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyExams.Models;
using System.Linq.Expressions;

namespace MyExams.Services
{
   public class TestService: ITestService
    {
        private readonly ITestRepository _testRepository;
        private readonly IGTestRepository _gTestRepository;
        private readonly IGAnswerSheetRepository _gAnswerSheetRepository;
        private readonly IGQuestionService _gQuestionService;
        private readonly ISectionService _sectionService;
        private readonly IQuestionService _questionService;
        private readonly IAnswerService _answerService;
        private readonly IGAnswerService _gAnswerService;
        public TestService(ITestRepository testRepository, IGTestRepository  gTestRepository, IGAnswerSheetRepository gAnswerSheetRepository, IGQuestionService gQuestionService, ISectionService sectionService, IAnswerService answerService, IQuestionService questionService, IGAnswerService gAnswerService)
        {
            _testRepository = testRepository;
            _gTestRepository = gTestRepository;
            _gAnswerSheetRepository = gAnswerSheetRepository;
            _gQuestionService = gQuestionService;
            _sectionService = sectionService;
            _questionService = questionService;
            _answerService = answerService;
            _gAnswerService = gAnswerService;
    }

        public void AddNewTest(Test item)
        {
            item.RecentUsage = DateTime.Now;
            _testRepository.Add(item);
            _testRepository.SaveChanges();
        }

        public IEnumerable<Test> GetAllTests()
        {
            return _testRepository.GetAll();
        }
        public GTest GetGTestBy(GAnswerSheet answerSheet)
        {
           return _gAnswerSheetRepository.Include(x => x.GTest).Where(x => x.Id == answerSheet.Id).Select(x=>x.GTest).FirstOrDefault();
        }
        public IEnumerable<GTest> GetGTestBy(int classId)
        {
            return _gTestRepository.IncludeAll().Where(x=>x.Class?.Id == classId && x.IsDone).ToList();
        }
        public IEnumerable<object> GetTestObjects<TKey>(string teacherId, Expression<Func<Test, TKey>> orderBy, OrderByMethod orderByMethod)
        {

            var tests = _testRepository.Where(x => x.Teacher.UserId == teacherId).AsQueryable();
            switch (orderByMethod)
            {
                case OrderByMethod.Ascending:
                    tests = tests.OrderBy(orderBy);
                    break;
                case OrderByMethod.Descending:
                    tests = tests.OrderByDescending(orderBy);
                    break;
            }
               
           
            return TestsListToObjects(tests.ToList());
        }
        public IEnumerable<object> GetTestObjects(string teacherId)
        {
            var tests = _testRepository.Where(x => x.Teacher.UserId == teacherId).ToList();
            return TestsListToObjects(tests);
        }
        private IEnumerable<object> TestsListToObjects(IEnumerable<Test> tests)
        {
            List<object> testsResult = new List<object>();
            foreach (var item in tests)
            {
                if (item.TestTitle == ""||item.TestTitle==null)
                {
                    item.TestTitle = "Неозаглавен тест";
                }
                testsResult.Add(new { testTitle = item.TestTitle, students = item.Students, averageMark = item.AverageMark, testCode = item.UniqueNumber });
            }
            return testsResult;
        }
        public Test GetTestByUniqueNumber(string uniqueNumber)
        {
            _testRepository.ClearCache();
            var test = _testRepository.GetAll().Where(x => x.UniqueNumber == uniqueNumber).FirstOrDefault(); ;
            if (test != null)
            {
                test.RecentUsage = DateTime.Now;
            }
            _testRepository.SaveChanges();
            return _testRepository.GetAll().Where(x => x.UniqueNumber == uniqueNumber).FirstOrDefault();
        }
        public IEnumerable<GTest> GetAllGTests()
        {
            return _gTestRepository.GetAll();
        } 
        public IEnumerable<GTest> GetAllGTestIncludeAll()
        {
            return _gTestRepository.IncludeAll();
        }
        public List<object> GetAnalysisBy(List<int> gTestIds, Test test)
        {
            var gQuestions = _gQuestionService.GetAllBy(gTestIds);
            var sections = _sectionService.GetAllSectionsByTestId(test.Id);
            var questionsObj = new List<object>();
            foreach (var section in sections)
            {
                var questions = _questionService.GetAllQuestionsBy(test.Id, section.OrderNo).ToList();

                foreach (var item in questions)
                {
                    var gQuestionsMatched = gQuestions.Where(x => x.Question.Id == item.Id);
                    double points = gQuestionsMatched.Sum(x => x.ReceivedPoints);
                    double totalPoints = gQuestionsMatched.Sum(x => x.Question.Points);
                    double percentage = Math.Round(points / totalPoints * 100);
                    if (item.QuestionType == QuestionType.Choice)
                    {
                        var answers = _answerService.GetAllBy(test.Id, section.OrderNo, item.OrderNo).ToList();

                        List<object> answersObj = new List<object>();
                        foreach (var answer in answers)
                        {
                            var gAnswers = _gAnswerService.GetAllBy(answer.Id);
                            double checkedCount = gAnswers.Count(x => x.CheckState == CheckState.Checked || x.CheckState == CheckState.Correct);
                            double totalCount = gAnswers.Count(x => x.CheckState != CheckState.NoInfo);
                            double checkedPercentage = Math.Round(checkedCount / totalCount * 100);
                            answersObj.Add(new { text = answer.Text, id = answer.OrderNo, percentage = checkedPercentage });
                        }
                        questionsObj.Add(new { text = item.Text, percentage = percentage, answers = answersObj });
                    }
                    else
                    {
                        questionsObj.Add(new { text = item.Text, percentage = percentage });
                    }
                }
                
            }
            return questionsObj;
        }
        public void AddNewGTest(GTest item)
        {
            _gTestRepository.Add(item);
            _gTestRepository.SaveChanges();
        }
        public void ClearTestCache()
        {
            _gTestRepository.ClearCache();
            
        }
        public void ClearGTestCache()
        {
            _testRepository.ClearCache();
        }
        public void Update()
        {
            _testRepository.SaveChanges();
        }
    }
}
