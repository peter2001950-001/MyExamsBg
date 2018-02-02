using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Models
{
    public class GAnswer
    {
        public int Id { get; set; }
        public GQuestion GQuestion { get; set; }
        public Answer Answer { get; set; }
        public int OrderNo { get; set; }
    }
}
