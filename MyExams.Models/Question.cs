using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }
        public Section Section { get; set; }
        public QuestionType QuestionType { get; set; }
        public string Text { get; set; }
        public int OrderNo { get; set; }
        public string CorrectAnswer { get; set; }
        public QuestionAnswerSize QuestionAnswerSize { get; set; }
        public bool Active { get; set; }
        public bool IsInUse { get; set; }
        public bool MixupAnswers { get; set; }
        public int Points { get; set; }

    }

    public enum QuestionType
    {
        Choice, 
        Text
    }
    public enum QuestionAnswerSize
    {
        Small,
        Medium,
        Long
    }
}
