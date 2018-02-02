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
        IEnumerable<Test> GetAll();
        Test GetTestByUniqueNumber(string uniqueNumber);
        void Update();
    }
}
