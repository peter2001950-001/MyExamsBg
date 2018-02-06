using MyExams.Models;
using MyExams.TestProcessing.TestClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MyExams.TestProcessing.Contracts
{
    public interface ITestGeneration
    {
        FileContentResult GenerateFile(Test test, List<Class> classes);

    }
}
