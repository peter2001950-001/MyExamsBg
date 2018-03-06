using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    /// <summary>
    /// All local classes that would be used during the generating process;
    /// TP (TestProcessing) is used to make the difference between classes in MyExam.Models and these classes below
    /// </summary>
namespace MyExams.TestProcessing.TestClasses
{
    
    class TPTest
    {
        public int GTestId { get; set; }
        public int TestId { get; set; }
        public string Title { get; set; }
        public TPStudentDetails StudentDetails { get; set; }
        public int MaxPoints { get; set; }
        public List<TPSection> Sections { get; set; }

    }

    class TPStudentDetails
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }
        public int NoInClass { get; set; }
        public string ClassName { get; set; }
    }
    class TPSection
    {
        public int SectionId { get; set; }
        public string Title { get; set; }
        public List<TPQuestion> Questions { get; set; }
    }

    class TPQuestion
    {
        public int QuestionId { get; set; }
        public QuestionType Type { get; set; }
        public QuestionAnswerSize AnswerSize { get; set; }
        public string Title { get; set; }
        public int maxPoints { get; set; }
        public List<TPAnswers> Answers { get; set; }
    }

    class TPAnswers
    {
        public int AnswerId { get; set; }
        public string Text { get; set; }
    }
}
