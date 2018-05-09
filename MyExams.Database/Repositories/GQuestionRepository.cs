using MyExams.Database.Contracts;
using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Database.Repositories
{
    public class GQuestionRepository : RepositoryBase<GQuestion>, IGQuestionRepository
    {
        public GQuestionRepository(IDatabase database) : base(database)
        {
        }
        public IEnumerable<GQuestion> WhereIncludeAll(Expression<Func<GQuestion, bool>> where)
        {
            return _dbSet.Where(where).Include(x => x.GAnswers).Include(x => x.GAnswers.Select(p=>p.Answer)).Include(x => x.Question).Include(x => x.Section);

        }
    }
}
