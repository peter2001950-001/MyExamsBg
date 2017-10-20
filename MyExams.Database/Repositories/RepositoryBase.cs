using MyExams.Database.Contracts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Database.Repositories
{
    public class RepositoryBase<T>:IRepositoryBase<T> where T : class 
    {
        private IDatabase _database;
        protected DbSet<T> _dbSet;
        protected IQueryable<T> _dbList;
        public RepositoryBase(IDatabase database)
        {
            _database = database;
            _dbSet = _database.Set<T>();
            _dbList = _dbSet.AsQueryable();
        }
        public void Add(T item)
        {
            _dbSet.Add(item);
           
        }
        public void Remove(T item)
        {
            _dbSet.Remove(item);
        }
        public IEnumerable<T> GetAll()
        {
            return _dbList.ToList();
        }
        public IEnumerable<T> Where(Expression<Func<T, bool>> where)
        {
            return _dbList.Where(where);
        }
       
        public void SaveChanges()
        {
            _database.SaveChanges();
        }
    }
}
