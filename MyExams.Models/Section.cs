using System.ComponentModel.DataAnnotations;
namespace MyExams.Models
{
   public class Section
    {
        [Key]
        public int Id { get; set; }
        public Test Test { get; set; }
        public string SectionTitle { get; set; }
        public int OrderNo { get; set; }
        public bool Active { get; set; }
        public bool IsInUse { get; set; }
        public bool MixupQuestions { get; set; }
        public int QuestionsToShow { get; set; }
        public string ImageFileName { get; set; }
    }
}
