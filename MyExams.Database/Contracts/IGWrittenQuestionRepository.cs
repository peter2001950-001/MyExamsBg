using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Database.Contracts
{
    public interface IGWrittenQuestionRepository: IRepositoryBase<GWrittenQuestion>
    {
        GWrittenQuestion GetWrittenQuestionBy(int gTestId, int orderNo);
        IEnumerable<GWrittenQuestion> GetWrittenQuestionBy(int gTestId);

    }
}
