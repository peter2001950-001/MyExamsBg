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
        [Key]
        public int Id { get; set; }
        public GTest GTest { get; set; }
        public Question Question { get; set; }
        public int OrderNo { get; set; }
    }
}
