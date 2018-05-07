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
        public virtual void Add(T item)
        {
            _dbSet.Add(item);
           
        }
    
        public virtual void Remove(T item)
        {
            _dbSet.Remove(item);
        }
        public virtual IEnumerable<T> GetAll()
        {
            return _dbList.AsEnumerable();
        }
        public virtual IEnumerable<T> Where(Expression<Func<T, bool>> where)
        {
            return _dbList.Where(where);
        }
        public virtual IEnumerable<T> Include<TKey>(Expression<Func<T, TKey>> expression)
        {
            return _dbSet.Include<T, TKey>(expression).ToList();
        }
       
        public virtual void SaveChanges()
        {
            _database.SaveChanges();
            _dbList = _dbSet.AsQueryable();
        }
    }
}
