using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Services.Contracts
{
    public interface IQuestionService
    {
        void AddQuestion(Question question);
        IEnumerable<Question> GetAll();
        IEnumerable<Question> GetAllQuestionsBy(int testId, int sectionNo);
        void Update(Question question);
        int GetPoints(int id);
        void RemoveQuestion(Question question);
        Question GetById(int id);
        IEnumerable<Question> GetAllByIds(List<int> ids);
        Question QuestionHasChanged(ParseQuestionClass questionClass, Question question);


    }
}
