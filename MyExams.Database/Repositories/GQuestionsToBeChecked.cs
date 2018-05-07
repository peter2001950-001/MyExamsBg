using MyExams.Database.Contracts;
using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Web;

namespace MyExams.Database.Repositories
{
    public class GQuestionsToBeCheckedRepository : RepositoryBase<GQuestionToBeChecked>, IGQuestionsToBeCheckedRepository
    {
        private static readonly object CacheLockObject = new object();
        private string cacheString ="GQuestionsToBeCheckedRepository-GetQuestionsToBeCheckedBy";
        public GQuestionsToBeCheckedRepository(IDatabase database) : base(database)
        {
        }
        public override IEnumerable<GQuestionToBeChecked> Where(Expression<Func<GQuestionToBeChecked, bool>> where)
        {
            return _dbSet.Where(where).Include(x => x.GWrittenQuestion).Include(x=>x.GWrittenQuestion.GTest);
        }
       
        public override void ClearCache()
        {
            HttpRuntime.Cache.Remove(cacheString);
            base.ClearCache();
        }
    }
}
