using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Services.Contracts
{
   public interface IGAnswerService
    {
        IEnumerable<GAnswer> GetAllBy(List<int> answerIds);

        IEnumerable<GAnswer> GetAll();
        IEnumerable<GAnswer> GetAllBy(int answerId);
    }
}
