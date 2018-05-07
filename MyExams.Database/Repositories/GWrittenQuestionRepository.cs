using MyExams.Database.CachedRepositories;
using MyExams.Database.Contracts;
using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyExams.Database.Repositories
{
    public class GWrittenQuestionRepository : CachedRepositoryBase<GWrittenQuestion>, IGWrittenQuestionRepository
    {
        private static readonly object CacheLockObject = new object();
        private string cacheString = "WrittenQuestion-GTest";
        public GWrittenQuestionRepository(IDatabase database) : base(database)
        {
        }
<<<<<<< HEAD

        public GWrittenQuestion GetWrittenQuestionBy(int gTestId, int orderNo)
        {
            var result = GetWrittenQuestionBy(gTestId).Where(x => x.GQuestionId == orderNo).FirstOrDefault();
            return result;
        }
        public IEnumerable<GWrittenQuestion> GetWrittenQuestionBy(int gTestId)
        {
            var result = HttpRuntime.Cache[cacheString] as List<GWrittenQuestionGTestId>;
            if (result == null)
            {
                lock (CacheLockObject)
                {
                    result = HttpRuntime.Cache[cacheString] as List<GWrittenQuestionGTestId>;
                    if (result == null)
                    {
                        result = _dbSet.Select(x => new GWrittenQuestionGTestId() { GTestId = x.GTest.Id, GWrittenQuestion = x }).ToList();
                        HttpRuntime.Cache.Insert(cacheString, result, null, DateTime.Now.AddSeconds(60), TimeSpan.Zero);

                    }
                }
            }
            return result.Select(x => x.GWrittenQuestion);
        }
        private class GWrittenQuestionGTestId
        {
            public int GTestId { get; set; }
            public GWrittenQuestion GWrittenQuestion { get; set; }
        }
        public override void ClearCache()
        {
            HttpRuntime.Cache.Remove(cacheString);
            base.ClearCache();
=======
        public IEnumerable<GWrittenQuestion> GetWrittenQuestionsBy(int gTestId)
        {
           return _dbSet.Where(x => x.GTest.Id == gTestId).Include(x => x.GTest);
>>>>>>> HEAD@{3}
        }
    }
}
