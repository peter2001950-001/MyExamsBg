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
        public TestService(ITestRepository testRepository)
        {
            _testRepository = testRepository;
        }

        public void AddNewTest(Test item)
        {
            _testRepository.Add(item);
            _testRepository.SaveChanges();
        }

        public IEnumerable<Test> GetAll()
        {
            return _testRepository.GetAll();
        }

        public Test GetTestByUniqueNumber(string uniqueNumber)
        {
            return _testRepository.GetAll().Where(x => x.UniqueNumber == uniqueNumber).FirstOrDefault();
        }
    }
}
