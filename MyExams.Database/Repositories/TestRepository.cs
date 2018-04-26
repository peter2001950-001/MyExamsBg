using MyExams.Database.CachedRepositories;
using MyExams.Database.Contracts;
using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Database.Repositories
{
    public class TestRepository : CachedRepositoryBase<Test>, ITestRepository
    {
        public TestRepository(IDatabase database) : base(database)
        {
        }
    }
}
