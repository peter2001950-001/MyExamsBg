
using MyExams.Database.Contracts;
using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Database.Repositories
{
    public class TeacherRepository : RepositoryBase<Teacher>, ITeacherRepository
    {
        public TeacherRepository(IDatabase database) : base(database)
        {
        }

        public Teacher GetTeacherByUserId(string userId)
        {
           return this.Where(t => t.UserId == userId).FirstOrDefault();
        }
    }
}
