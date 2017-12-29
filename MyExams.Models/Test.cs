using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Models
{
    public class Test
    {
        public Test()
        {
            UniqueNumber = Guid.NewGuid().ToString("N");
        }
        [Key]
        public int Id { get; set; }
        public string UniqueNumber { get; set; }
        public Teacher Teacher { get; set; }
        public string TestTitle { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
