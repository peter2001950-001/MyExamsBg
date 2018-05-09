using Hangfire;
using Hangfire.Dashboard;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using MyExams.Database;
using MyExams.Database.Contracts;
using MyExams.Database.Repositories;
using MyExams.Services;
using MyExams.Services.Contracts;
using MyExams.TestProcessing;
using MyExams.TestProcessing.Contracts;
using Ninject;
using Ninject.Web.Common;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyExams.Startup))]
namespace MyExams
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var kernel = new StandardKernel();
            kernel
               .Bind<IDatabase>()
               .To<Database.Database>()
           .InBackgroundJobScope();

            kernel
                .Bind<IStudentRepository>()
                .To<StudentRepository>()
                .InBackgroundJobScope();
            kernel
              .Bind<ITeacherRepository>()
              .To<TeacherRepository>()
              .InBackgroundJobScope();
            kernel
              .Bind<IStudentService>()
              .To<StudentService>()
              .InBackgroundJobScope();
            kernel
              .Bind<IClassRepository>()
              .To<ClassRepository>()
              .InBackgroundJobScope();
            kernel
             .Bind<IStudentClassRepository>()
             .To<StudentClassRepository>()
             .InBackgroundJobScope();
            kernel
            .Bind<ITestRepository>()
            .To<TestRepository>()
            .InBackgroundJobScope();
            kernel
            .Bind<ISectionRepository>()
            .To<SectionRepository>()
            .InBackgroundJobScope();
            kernel
            .Bind<IQuestionRepository>()
            .To<QuestionRepository>()
            .InBackgroundJobScope();
            kernel
            .Bind<IAnswerRepository>()
            .To<AnswerRepository>()
            .InBackgroundJobScope();
            kernel
            .Bind<IGTestRepository>()
            .To<GTestRepository>()
            .InBackgroundJobScope();
            kernel
           .Bind<IGAnswerSheetRepository>()
           .To<GAnswerSheetRepository>()
           .InBackgroundJobScope();
            kernel
           .Bind<IGWrittenQuestionRepository>()
           .To<GWrittenQuestionRepository>()
           .InBackgroundJobScope();
            kernel
           .Bind<IGQuestionsToBeCheckedRepository>()
           .To<GQuestionsToBeCheckedRepository>()
           .InBackgroundJobScope();
            kernel
          .Bind<IUploadSessionRepository>()
          .To<UploadSessionRepository>()
          .InRequestScope();
            kernel
        .Bind<IFileDirectoryRepository>()
        .To<FileDirectoryRepository>()
        .InRequestScope();
            kernel
        .Bind<IUploadSessionFileDirectoryRepository>()
        .To<UploadSessionFileDirectoryRepository>()
        .InRequestScope();
            kernel
       .Bind<IGQuestionRepository>()
       .To<GQuestionRepository>()
       .InRequestScope();
            kernel
       .Bind<IGAnswerRepository>()
       .To<GAnswerRepository>()
       .InRequestScope();
            kernel
     .Bind<IGAnswerService>()
     .To<GAnswerService>()
     .InRequestScope();






            kernel
            .Bind<IClassService>()
            .To<ClassService>()
            .InBackgroundJobScope();
            kernel
              .Bind<ITeacherService>()
              .To<TeacherService>()
              .InBackgroundJobScope();
            kernel
            .Bind<ITestService>()
            .To<TestService>()
            .InBackgroundJobScope();
            kernel
            .Bind<ISectionService>()
            .To<SectionService>()
            .InBackgroundJobScope();
            kernel
            .Bind<IQuestionService>()
            .To<QuestionService>()
            .InBackgroundJobScope();
            kernel
            .Bind<IAnswerService>()
            .To<AnswerService>()
            .InBackgroundJobScope();
            kernel
            .Bind<IGAnswerSheetService>()
            .To<GAnswerSheetService>()
            .InBackgroundJobScope();
            kernel
           .Bind<IUploadSessionService>()
           .To<UploadSessionService>()
           .InRequestScope();
            kernel
           .Bind<IFileDirectoryService>()
           .To<FileDirectoryService>()
           .InRequestScope();
            kernel
          .Bind<IGQuestionService>()
          .To<GQuestionService>()
          .InRequestScope();



            kernel
           .Bind<ITestGeneration>()
           .To<TestGeneration>()
           .InBackgroundJobScope();

            kernel
           .Bind<ITestCheckProcess>()
           .To<TestCheckProcess>()
           .InRequestScope();

            GlobalConfiguration.Configuration.UseNinjectActivator(kernel);
            ConfigureAuth(app);
            GlobalConfiguration.Configuration
               .UseSqlServerStorage("DefaultConnection");
            app.UseHangfireDashboard();
            app.UseHangfireServer();

           


        }
        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddScoped<ITestService, TestService>();
        //    services.AddScoped<IGAnswerSheetService, GAnswerSheetService>();
        //    services.AddScoped<IQuestionService, QuestionService>();
        //    services.AddScoped<IAnswerService, AnswerService>();
        //    services.AddScoped<ITeacherService, TeacherService>();
        //    services.AddScoped<IClassService, ClassService>();
        //    services.AddScoped<ISectionService, SectionService>();
        //    services.AddScoped<IStudentService, StudentService>();

        //    services.AddScoped<IAnswerRepository, AnswerRepository>();
        //    services.AddScoped<IClassRepository, ClassRepository>();
        //    services.AddScoped<IGTestRepository, GTestRepository>();
        //    services.AddScoped<IGAnswerSheetRepository, GAnswerSheetRepository>();
        //    services.AddScoped<IGQuestionsToBeCheckedRepository, GQuestionsToBeCheckedRepository>();
        //    services.AddScoped<ITestRepository, TestRepository>();
        //    services.AddScoped<IGWrittenQuestionRepository, GWrittenQuestionRepository>();
        //    services.AddScoped<IQuestionRepository, QuestionRepository>();
        //    services.AddScoped<ISectionRepository, SectionRepository>();
        //    services.AddScoped<IStudentClassRepository, StudentClassRepository>();
        //    services.AddScoped<ITeacherRepository, TeacherRepository>();

        //    services.AddScoped<IDatabase,MyExams.Database.Database>();
        //}

        }

    
}
