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

      
    }
}
