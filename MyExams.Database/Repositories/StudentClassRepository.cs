using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using MyExams.Database.Contracts;

namespace MyExams.Database.Repositories
{
    public class StudentClassRepository: RepositoryBase<StudentClass>, IStudentClassRepository
    {
        public StudentClassRepository(IDatabase _database)
            :base(_database)
        {
            this._dbList = _dbSet.Include(x => x.Student).Include(x => x.Class).AsQueryable();
        }

        public override void SaveChanges()
        {

            base.SaveChanges();
            this._dbList = _dbSet.Include(x => x.Student).Include(x => x.Class).AsQueryable();
        }
    }
}
