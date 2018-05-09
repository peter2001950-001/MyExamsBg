using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Models
{
   public class GQuestion
    {
        public GQuestion()
        {
            GAnswers = new List<GAnswer>();
        }
        [Key]
        public int Id { get; set; }
        public Question Question { get; set; }
        public Section Section { get; set; }
        public GTest GTest { get; set; }
        public GAnswerSheet GAnswerSheet { get; set; }
        public int OrderNo { get; set; }
        public int ReceivedPoints { get; set; }
        public ICollection<GAnswer> GAnswers { get; set; }
    }
}
