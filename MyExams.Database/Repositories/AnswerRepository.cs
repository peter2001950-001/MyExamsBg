using MyExams.Database.Contracts;
using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Database.Repositories
{
    public class AnswerRepository : RepositoryBase<Answer>, IAnswerRepository
    {
        public AnswerRepository(IDatabase database) : base(database)
        {
        }
        public bool GetIsCorrect(int id)
        {
            return _dbSet.Where(x => x.Id == id).Select(x => x.IsCorrect).FirstOrDefault();
        }
        public void Update(Answer answer)
        {
            var item = _dbSet.Where(x => x.Id == answer.Id).FirstOrDefault();
            if (item != null)
            {
                item.Active = answer.Active;
                item.IsCorrect = answer.IsCorrect;
                item.IsInUse = answer.IsInUse;
                item.OrderNo = answer.OrderNo;
                item.Text = answer.Text;
            }
            SaveChanges();
        }
    }
}
