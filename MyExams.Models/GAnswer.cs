using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Models
{
    public class GAnswer
    {
        [Key]
        public int Id { get; set; }
        public Answer Answer { get; set; }
        public int OrderNo { get; set; }
        public CheckState CheckState { get; set; }
    }
    public enum CheckState
    {
        NotChecked,
        Checked,
        Correct, 
        NoInfo
    }
}
