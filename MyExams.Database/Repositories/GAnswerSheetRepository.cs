using MyExams.Database.Contracts;
using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Database.Repositories
{
    public class GAnswerSheetRepository : RepositoryBase<GAnswerSheet>, IGAnswerSheetRepository
    {
        public GAnswerSheetRepository(IDatabase database) : base(database)
        {
        }
        public IEnumerable<GAnswerSheet> GetGAnswerSheetsBy(int gTestId)
        {
            return _dbSet.Where(x => x.GTest.Id == gTestId).Include(x => x.GTest);
        }
    }
}
