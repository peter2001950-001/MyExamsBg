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
        public TestService(ITestRepository testRepository, IGTestRepository  gTestRepository, IGAnswerSheetRepository gAnswerSheetRepository)
        {
            _testRepository = testRepository;
            _gTestRepository = gTestRepository;
            _gAnswerSheetRepository = gAnswerSheetRepository;
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
        public void AddNewGTest(GTest item)
        {
            _gTestRepository.Add(item);
            _gTestRepository.SaveChanges();
        }
        public void Update()
        {
            _testRepository.SaveChanges();
        }
    }
}
