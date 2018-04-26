using MyExams.Database.CachedRepositories;
using MyExams.Database.Contracts;
using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyExams.Database.Repositories
{
    public class GAnswerSheetRepository : CachedRepositoryBase<GAnswerSheet>, IGAnswerSheetRepository
    {
        private static readonly object CacheLockObject = new object();
        private string cacheStringGetAll = "GAnswerSheet-WhereIncludeAll-";
        public GAnswerSheetRepository(IDatabase database) : base(database)
        {
        }
        public IEnumerable<GAnswerSheet> WhereIncludeAll(Expression<Func<GAnswerSheet, bool>> where)
        {
            cacheStringGetAll += where.ToString();
            var result = HttpRuntime.Cache[cacheStringGetAll] as List<GAnswerSheet>;
            if (result == null)
            {
                lock (CacheLockObject)
                {
                    result = HttpRuntime.Cache[cacheStringGetAll] as List<GAnswerSheet>;
                    if (result == null)
                    {
                        result = _dbSet.Where(where).Include(x => x.GTest).ToList();
                        HttpRuntime.Cache.Insert(cacheStringGetAll, result, null, DateTime.Now.AddSeconds(60), TimeSpan.Zero);

                    }
                }
            }
            return result;
        }
    }
}
