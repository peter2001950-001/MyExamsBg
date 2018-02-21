using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using MyExams.Models;

namespace MyExams.Database
{
    public class Database : DbContext, IDatabase
    {
        public Database() : base("DefaultConnection")
        {
        }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<StudentClass> StudentsClases { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<GTest> GTests { get; set; }
        public DbSet<GAnswerSheet> GAnswerSheets { get; set; }
        public DbSet<GWrittenQuestion> GWrittenQuestions { get; set; }
        public DbSet<GQuestionToBeChecked> GQuestionsToBeChecked { get; set; }
        public DbSet<FileDirectory> FileDirectories {get; set;}
        public DbSet<UploadSession> UploadSessions { get; set; }
        public DbSet<UploadSessionFileDirectory> UploadSessionsFileDirectories { get; set; }
        public void SaveChanges()
        {
            base.SaveChanges();
        }

    }
}
