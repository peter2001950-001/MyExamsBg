using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Models
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }
        public Question Question { get; set; }
        public string Text { get; set; }
        public int OrderNo { get; set; }
        public bool IsCorrect { get; set; }
        public bool IsInUse { get; set; }
        public bool Active { get; set; }
    }
}
