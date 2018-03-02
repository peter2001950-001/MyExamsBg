using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Services.Contracts
{
    public interface ITestService
    {
        void AddNewTest(Test item);
        IEnumerable<Test> GetAllTests();
        IEnumerable<object> GetTestObjects<TKey>(string teacherId, Expression<Func<Test, TKey>> orderBy, OrderByMethod orderByMethod);
        IEnumerable<object> GetTestObjects(string teacherId);
        Test GetTestByUniqueNumber(string uniqueNumber);
        IEnumerable<GTest> GetAllGTests();
        void AddNewGTest(GTest item);
        void Update();
    }
}
