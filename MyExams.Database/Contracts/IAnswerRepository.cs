using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Database.Contracts
{
    public interface IAnswerRepository:IRepositoryBase<Answer>
    {
        bool GetIsCorrect(int id);
        void Update(Answer answer);
    }
}
