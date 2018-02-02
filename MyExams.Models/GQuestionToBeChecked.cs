using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Models
{
   public class GQuestionToBeChecked
    {
        [Key]
        public int Id { get; set; }
        public Teacher Teacher { get; set; }
        public GWrittenQuestion GWrittenQuestion { get; set; }
    }
}
