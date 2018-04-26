using MyExams.Database.Contracts;
using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data.Entity;
using MyExams.Database.CachedRepositories;
using System.Web;

namespace MyExams.Database.Repositories
{
    public class GQuestionsToBeCheckedRepository : CachedRepositoryBase<GQuestionToBeChecked>, IGQuestionsToBeCheckedRepository
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
        public IEnumerable<GQuestionToBeChecked> GetQuestionsToBeCheckedBy(int teacherId)
        {
            var result = HttpRuntime.Cache[cacheString] as List<GQuestionToBeChecked>;
            if (result == null)
            {
                lock (CacheLockObject)
                {
                    result = HttpRuntime.Cache[cacheString] as List<GQuestionToBeChecked>;
                    if (result == null)
                    {
                        result = _dbSet.Where(x => x.Teacher.Id == teacherId).Include(x => x.GWrittenQuestion).Include(x => x.GWrittenQuestion.GTest).ToList();
                        HttpRuntime.Cache.Insert(cacheString, result, null, DateTime.Now.AddSeconds(60), TimeSpan.Zero);

                    }
                }
            }
            return result;
        }
        public override void ClearCache()
        {
            HttpRuntime.Cache.Remove(cacheString);
            base.ClearCache();
        }
    }
}
