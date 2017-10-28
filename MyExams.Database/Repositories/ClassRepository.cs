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
        private readonly IDatabase _database;
        public ClassRepository(IDatabase database)
            :base(database)
        {
            _database = database;
            this._dbList = this._dbSet.Include(x => x.Teacher).AsQueryable() ;
        }

        public override void SaveChanges()
        {
            _database.SaveChanges();
            this._dbList = this._dbSet.Include(x => x.Teacher).AsQueryable();
        }
      
    }
}
