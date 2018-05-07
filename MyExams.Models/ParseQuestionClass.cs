using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Models
{
    public class ParseQuestionClass
    {
        public string testCode { get; set; }
        public int sectionId { get; set; }
        public Question question { get; set; }
        public class Question
        {
            public int id { get; set; }
            public string text { get; set; }
            public List<Option> options { get; set; }
            public string correctAnswer { get; set; }
            public string selectedAnswerSize { get; set; }
            public bool mixupOptions { get; set; }
            public int points { get; set; }
            public class Option
            {
                public int id { get; set; }
                public string text { get; set; }
                public bool isCorrect { get; set; }
            }
        }
    }
}
