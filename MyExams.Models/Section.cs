using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Models
{
   public class Section
    {
        [Key]
        public int Id { get; set; }
        public Test Test { get; set; }
        public string SectionTitle { get; set; }
        public int OrderNo { get; set; }
        public bool Active { get; set; }
        public bool IsInUse { get; set; }
        public bool MixupQuestions { get; set; }
    }
}
