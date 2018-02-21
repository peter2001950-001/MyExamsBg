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
   public class UploadSessionFileDirectoryRepository : RepositoryBase<UploadSessionFileDirectory>, IUploadSessionFileDirectoryRepository
    {
        public UploadSessionFileDirectoryRepository(IDatabase database) : base(database)
        {
            this._dbList = _dbSet.Include(x => x.FileDirectory).Include(x => x.UploadSession);
        }
    }
}
