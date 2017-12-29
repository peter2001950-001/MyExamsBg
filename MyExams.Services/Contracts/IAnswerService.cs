using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Services.Contracts
{
    public interface IAnswerService
    {
        IEnumerable<Answer> GetAll();
        IEnumerable<Answer> GetAllBy(int testId, int sectionNo, int questionNo);
        void AddAnswer(Answer answer);
    }
}
