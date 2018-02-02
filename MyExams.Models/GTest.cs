using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Models
{
   public class GTest
    {
        [Key]
        public int Id { get; set; }
        public Teacher Teacher { get; set; }
        public Student Student { get; set; }
        public Test Test { get; set; }
        public int MaxPoints { get; set; }
        public int ReceivedPoints { get; set; }
    }
}
