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
    public class GQuestionsToBeCheckedRepository : RepositoryBase<GQuestionToBeChecked>, IGQuestionsToBeCheckedRepository
    {
        public GQuestionsToBeCheckedRepository(IDatabase database) : base(database)
        {
        }
        public override IEnumerable<GQuestionToBeChecked> Where(Expression<Func<GQuestionToBeChecked, bool>> where)
        {
            return _dbSet.Where(where).Include(x => x.GWrittenQuestion).Include(x=>x.GWrittenQuestion.GTest);
        }
    }
}
