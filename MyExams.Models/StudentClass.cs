using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Models
{
   public class StudentClass
    {
        [Key]
        public int Id { get; set; }
        public Student Student { get; set; }
        public Class Class { get; set; }
        public int NoInClass { get; set; }
        
    }
}
