using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Models
{
   public class Class
    {
        [Key]
        public int Id { get; set; }
        public string UniqueCode { get; set; }
        public Teacher Teacher { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public int StudentsCount { get; set; }
        public double AverageMark { get; set; }
        public bool FirstMessageShowed { get; set; }
        public string ClassColor { get; set; }
        public DateTime RecentUsage { get; set; }
    }
}
