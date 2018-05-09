using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Database
{
    public interface IDatabase: IDisposable
    {

        DbSet<T> Set<T>() where T: class;
        DbSet<Teacher> Teachers { get; set; }
        DbSet<Class> Classes { get; set; }
        DbSet<StudentClass> StudentsClases { get; set; }
        DbSet<Student> Students { get; set; }
        DbSet<Test> Tests { get; set; }
        DbSet<Section> Sections { get; set; }
        DbSet<Question> Questions { get; set; }
        DbSet<Answer> Answers { get; set; }
        DbSet<GTest> GTests { get; set; }
        DbSet<GAnswerSheet> GAnswerSheets { get; set; }
        DbSet<GWrittenQuestion> GWrittenQuestions { get; set; }
        DbSet<GQuestionToBeChecked> GQuestionsToBeChecked { get; set; }
        void SaveChanges();
    }
}
