using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Database.Contracts
{
    public interface IUploadSessionFileDirectoryRepository:IRepositoryBase<UploadSessionFileDirectory>
    {
        IEnumerable<UploadSessionFileDirectory> WhereIncludeAll(Expression<Func<UploadSessionFileDirectory, bool>> where);
    }
    
}
