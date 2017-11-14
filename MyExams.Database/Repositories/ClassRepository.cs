using MyExams.Database.Contracts;
using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace MyExams.Database.Repositories
{
    public class ClassRepository: RepositoryBase<Class>, IClassRepository
    {
      
        public ClassRepository(IDatabase database)
            :base(database)
        {
           
            this._dbList = this._dbSet.Include(x => x.Teacher).AsQueryable() ;
        }
        public void Update(Class item)
        {
           var itemToUpdate = _dbSet.SingleOrDefault(c => c.Id == item.Id);
            if (itemToUpdate != null)
            {
                itemToUpdate = item;
            }

        }
        public override void SaveChanges()
        {
            base.SaveChanges();
            this._dbList = this._dbSet.Include(x => x.Teacher).AsQueryable();
        }
      
    }
}
