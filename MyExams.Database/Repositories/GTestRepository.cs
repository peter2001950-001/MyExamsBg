using MyExams.Database.Contracts;
using MyExams.Models;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyExams.Database.CachedRepositories;

namespace MyExams.Database.Repositories
{
    public class GTestRepository : CachedRepositoryBase<GTest>, IGTestRepository
    {
        public GTestRepository(IDatabase database) : base(database)
        {
        }
        public IEnumerable<GTest> IncludeAll()
        {
            return _dbSet.Include(x => x.Test).Include(x => x.Student).Include(x => x.Teacher).Include(x => x.Class);
        }
    }
}
