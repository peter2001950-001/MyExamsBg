using MyExams.Database.CachedRepositories;
using MyExams.Database.Contracts;
using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data.Entity;

namespace MyExams.Database.Repositories
{
    public class QuestionRepository : CachedRepositoryBase<Question>, IQuestionRepository
    {
        public QuestionRepository(IDatabase database) : base(database)
        {
        }
        public override IEnumerable<Question> Where(Expression<Func<Question, bool>> where)
        {
            return _dbSet.Where(where).Include(x => x.Section);
        }

        public int GetPoints(int id)
        {
             return  _dbSet.Where(x => x.Id == id).Select(x => x.Points).FirstOrDefault();
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
