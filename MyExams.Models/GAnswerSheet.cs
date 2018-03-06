using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Models
{
    public class GAnswerSheet
    {
        [Key]
        public int Id { get; set; }
        public GTest GTest { get; set; }
        public int PageNo { get; set; }
        public string Barcode { get; set; }
        public int FirstQuestionNo { get; set; }
        public int LastQuestionNo { get; set; }
        public bool IsUploaded { get; set; }
        public AnswerSheetStatus AnswerSheetStatus { get; set; }
        public FileDirectory ScannedFileName { get; set; }
        public FileDirectory CheckedFileName { get; set; }
        public string Xml { get; set; }
    }
    public enum AnswerSheetStatus
    {
        NotChecked,
        WaitingWrittenQuestionsConfirmation,
        Checked
    }
}
