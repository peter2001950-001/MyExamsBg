using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Services.Contracts
{
    public interface ITestService
    {
        void AddNewTest(Test item);
        IEnumerable<Test> GetAllTests();
        Test GetTestByUniqueNumber(string uniqueNumber);
        IEnumerable<GTest> GetAllGTests();
        void AddNewGTest(GTest item);
        void Update();
    }
}
