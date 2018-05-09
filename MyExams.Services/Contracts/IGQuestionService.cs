using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Services.Contracts
{
   public interface IGQuestionService
    {
        void AddNewGQuestion(GQuestion gQuestion);
        IEnumerable<GQuestion> GetAllBy(int gTestId);
        IEnumerable<GQuestion> GetAllBy(List<int> gTestIds);
    }
}
