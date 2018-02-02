using MyExams.Database.Contracts;
using MyExams.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyExams.Models;

namespace MyExams.Services
{
   public class TestService: ITestService
    {
        private readonly ITestRepository _testRepository;
        private readonly IGTestRepository _gTestRepository;
        public TestService(ITestRepository testRepository, IGTestRepository  gTestRepository)
        {
            _testRepository = testRepository;
            _gTestRepository = gTestRepository;
        }

        public void AddNewTest(Test item)
        {
            _testRepository.Add(item);
            _testRepository.SaveChanges();
        }

        public IEnumerable<Test> GetAllTests()
        {
            return _testRepository.GetAll();
        }

        public Test GetTestByUniqueNumber(string uniqueNumber)
        {
            return _testRepository.GetAll().Where(x => x.UniqueNumber == uniqueNumber).FirstOrDefault();
        }
        public IEnumerable<GTest> GetAllGTests()
        {
            return _gTestRepository.GetAll();
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
