using MyExams.Database.CachedRepositories;
using MyExams.Database.Contracts;
using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Database.Repositories
{
    public class StudentRepository : CachedRepositoryBase<Student>, IStudentRepository
    {
        public StudentRepository(IDatabase database) : base(database)
        {
        }

        public Student GetStudentByUserId(string userId)
        {
            return this.Where(s => s.UserId == userId).FirstOrDefault();
        }
    }
}
