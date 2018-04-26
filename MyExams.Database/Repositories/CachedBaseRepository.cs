using MyExams.Database.Contracts;
using MyExams.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MyExams.Database;

namespace MyExams.Database.CachedRepositories
{
    public abstract class CachedRepositoryBase<T> : MyExams.Database.Repositories.RepositoryBase<T> where T : class
    {
        private static readonly object CacheLockObject = new object();
        private string cacheStringGetAll = typeof(T).ToString() + "-GetAll";

        private string cacheStringInclude = typeof(T).ToString() + "-Include-";
        public CachedRepositoryBase(IDatabase database) : base(database)
        {
        }
        public override IEnumerable<T> GetAll()
        {
          
            var result = HttpRuntime.Cache[cacheStringGetAll] as List<T>;
            if (result == null)
            {
                lock (CacheLockObject)
                {
                    result = HttpRuntime.Cache[cacheStringGetAll] as List<T>;
                    if (result == null)
                    {
                        result = base.GetAll().ToList();
                        HttpRuntime.Cache.Insert(cacheStringGetAll, result, null, DateTime.Now.AddSeconds(60), TimeSpan.Zero);

                    }
                }
            }
            return result;
        }
        public override IEnumerable<T> Include<TKey>(System.Linq.Expressions.Expression<Func<T, TKey>> expression)
        {
            cacheStringInclude += expression.ToString();
            var result = HttpRuntime.Cache[cacheStringInclude] as IEnumerable<T>;
            if (result == null)
            {
                lock (CacheLockObject)
                {
                    result = HttpRuntime.Cache[cacheStringInclude] as IEnumerable<T>;
                    if (result == null)
                    {
                        result = base.Include(expression);
                        HttpRuntime.Cache.Insert(cacheStringInclude, result, null, DateTime.Now.AddSeconds(60), TimeSpan.Zero);

                    }
                }
            }
            return result;
        }
        public override void ClearCache()
        {
            HttpRuntime.Cache.Remove(cacheStringGetAll);
            HttpRuntime.Cache.Remove(cacheStringInclude);
        }
    }
}
