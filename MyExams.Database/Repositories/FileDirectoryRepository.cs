using MyExams.Database.Contracts;
using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Database.Repositories
{
    public class FileDirectoryRepository : RepositoryBase<FileDirectory>, IFileDirectoryRepository
    {
        public FileDirectoryRepository(IDatabase database) : base(database)
        {
        }
    }
}
