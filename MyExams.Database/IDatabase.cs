using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Database
{
    public interface IDatabase
    {

        DbSet<T> Set<T>() where T: class;
        DbSet<Teacher> Teachers { get; set; }
        DbSet<Class> Classes { get; set; }
         DbSet<StudentClass> StudentsClases { get; set; }
         DbSet<Student> Students { get; set; }
        void SaveChanges();
    }
}
