[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(MyExams.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(MyExams.App_Start.NinjectWebCommon), "Stop")]

namespace MyExams.App_Start
{
    using System;
    using System.Web;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using Database;
    using Database.Contracts;
    using Database.Repositories;
    using Services.Contracts;
    using Services;
    using TestProcessing.Contracts;
    using TestProcessing;
    using Hangfire;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {

            kernel
                .Bind<IDatabase>()
                .To<Database>()
            .InRequestScope();

            kernel
                .Bind<IStudentRepository>()
                .To<StudentRepository>()
                .InRequestScope();
            kernel
              .Bind<ITeacherRepository>()
              .To<TeacherRepository>()
              .InRequestScope();
            kernel
              .Bind<IStudentService>()
              .To<StudentService>()
              .InRequestScope();
            kernel
              .Bind<IClassRepository>()
              .To<ClassRepository>()
              .InRequestScope();
            kernel
             .Bind<IStudentClassRepository>()
             .To<StudentClassRepository>()
             .InRequestScope();
            kernel
            .Bind<ITestRepository>()
            .To<TestRepository>()
            .InRequestScope();
            kernel
            .Bind<ISectionRepository>()
            .To<SectionRepository>()
            .InRequestScope();
            kernel
            .Bind<IQuestionRepository>()
            .To<QuestionRepository>()
            .InRequestScope();
            kernel
            .Bind<IAnswerRepository>()
            .To<AnswerRepository>()
            .InRequestScope();
            kernel
            .Bind<IGTestRepository>()
            .To<GTestRepository>()
            .InRequestScope();
           
            kernel
           .Bind<IGAnswerSheetRepository>()
           .To<GAnswerSheetRepository>()
           .InRequestScope();
            kernel
           .Bind<IGWrittenQuestionRepository>()
           .To<GWrittenQuestionRepository>()
           .InRequestScope();
            kernel
           .Bind<IGQuestionsToBeCheckedRepository>()
           .To<GQuestionsToBeCheckedRepository>()
           .InRequestScope();
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
            .Bind<IClassService>()
            .To<ClassService>()
            .InRequestScope();
            kernel
              .Bind<ITeacherService>()
              .To<TeacherService>()
              .InRequestScope();
            kernel
            .Bind<ITestService>()
            .To<TestService>()
            .InRequestScope();
            kernel
            .Bind<ISectionService>()
            .To<SectionService>()
            .InRequestScope();
            kernel
            .Bind<IQuestionService>()
            .To<QuestionService>()
            .InRequestScope();
            kernel
            .Bind<IAnswerService>()
            .To<AnswerService>()
            .InRequestScope();
            kernel
            .Bind<IGAnswerSheetService>()
            .To<GAnswerSheetService>()
            .InRequestScope();
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
         .Bind<IGAnswerService>()
         .To<GAnswerService>()
         .InRequestScope();


            kernel
            .Bind<ITestGeneration>()
            .To<TestGeneration>()
            .InRequestScope();
            kernel
            .Bind<ITestCheckProcess>()
            .To<TestCheckProcess>()
            .InRequestScope();
        }
    }
}
