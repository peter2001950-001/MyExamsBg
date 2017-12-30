using MyExams.Database.Contracts;
using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Database.Repositories
{
    public class QuestionRepository : RepositoryBase<Question>, IQuestionRepository
    {
        public QuestionRepository(IDatabase database) : base(database)
        {
        }

        public void Update(Question question)
        {
            var item = _dbSet.Where(x => x.Id == question.Id).FirstOrDefault();
            if (item != null)
            {
                item.Active = question.Active;
                item.CorrectAnswer = question.CorrectAnswer;
                item.IsInUse = question.IsInUse;
                item.OrderNo = question.OrderNo;
                item.QuestionAnswerSize = question.QuestionAnswerSize;
                item.Text = item.Text;
                SaveChanges();
            }
        }
    }
}
