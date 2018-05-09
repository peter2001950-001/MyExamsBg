using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Database.Contracts
{
    public interface IGAnswerRepository:IRepositoryBase<GAnswer>
    {
        IEnumerable<GAnswer> GetAllBy(List<int> answerIds);
    }
}
