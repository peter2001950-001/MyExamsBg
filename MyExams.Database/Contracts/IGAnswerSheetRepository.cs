
using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Database.Contracts
{
    public interface IGAnswerSheetRepository:IRepositoryBase<GAnswerSheet>
    {
<<<<<<< HEAD
        IEnumerable<GAnswerSheet> WhereIncludeAll(Expression<Func<GAnswerSheet, bool>> where);
=======
        IEnumerable<GAnswerSheet> GetGAnswerSheetsBy(int gTestId);
>>>>>>> HEAD@{3}
    }
}
