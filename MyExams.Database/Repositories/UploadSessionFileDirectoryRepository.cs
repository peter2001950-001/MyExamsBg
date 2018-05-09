using MyExams.Database.Contracts;
using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Entity;
using System.Linq.Expressions;

namespace MyExams.Database.Repositories
{
   public class UploadSessionFileDirectoryRepository : RepositoryBase<UploadSessionFileDirectory>, IUploadSessionFileDirectoryRepository
    {
        public UploadSessionFileDirectoryRepository(IDatabase database) : base(database)
        {
            this._dbList = _dbSet.Include(x => x.FileDirectory).Include(x => x.UploadSession).Include(x=>x.AnswerSheet);
        }
        public override IEnumerable<UploadSessionFileDirectory> Include<TKey>(System.Linq.Expressions.Expression<Func<UploadSessionFileDirectory, TKey>> expression)
        {
            return _dbSet.Include(x=>x.FileDirectory).Include(expression);
        }
        public IEnumerable<UploadSessionFileDirectory> WhereIncludeAll(Expression<Func<UploadSessionFileDirectory, bool>> where)
        {
            return _dbSet.Where(where).Include(x => x.AnswerSheet).Include(x => x.FileDirectory).AsNoTracking();
        }
    }
}
