using MyExams.Database.Contracts;
using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Database.Repositories
{
    public class GQuestionRepository : RepositoryBase<GQuestion>, IGQuestionRepository
    {
        public GQuestionRepository(IDatabase database) : base(database)
        {
        }
    }
}
