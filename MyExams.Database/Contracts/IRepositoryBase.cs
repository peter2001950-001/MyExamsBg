using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Database.Contracts
{
    public interface IRepositoryBase<T> where T : class
    {
        void Add(T item);
        void Remove(T item);
        IEnumerable<T> GetAll();
        IEnumerable<T> Where(Expression<Func<T, bool>> where);
        void SaveChanges();

    }
}
