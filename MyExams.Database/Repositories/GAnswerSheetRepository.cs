
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
    public class GAnswerSheetRepository : RepositoryBase<GAnswerSheet>, IGAnswerSheetRepository
    {
        private static readonly object CacheLockObject = new object();
        public GAnswerSheetRepository(IDatabase database) : base(database)
        {
        }
        public IEnumerable<GAnswerSheet> WhereIncludeAll(Expression<Func<GAnswerSheet, bool>> where)
        {
           return _dbSet.Where(where).Include(x => x.GTest);
        }
        public IEnumerable<GAnswerSheet> GetGAnswerSheetsBy(int gTestId)
        {
            return _dbSet.Where(x => x.GTest.Id == gTestId).Include(x => x.GTest);
        }
    }
}
