using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Database.Contracts
{
   public  interface IGQuestionRepository:IRepositoryBase<GQuestion>
    {
        IEnumerable<GQuestion> WhereIncludeAll(Expression<Func<GQuestion, bool>> where);
    }
}
