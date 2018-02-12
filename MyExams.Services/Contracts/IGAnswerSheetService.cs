using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Services.Contracts
{
   public interface IGAnswerSheetService
    {
        IEnumerable<GAnswerSheet> GetAllGAnswerSheet();
        IEnumerable<GWrittenQuestion> GetAllGWrittenQuestions();
        IEnumerable<GQuestionToBeChecked> GetAllGQuestionToBeChecked();
        void AddGAnswerSheet(GAnswerSheet gAnswerSheet);
        void AddGWrittenQuestion(GWrittenQuestion gWrittenQuestion);
        void AddGQuestionToBeChecked(GQuestionToBeChecked gQuestionToBeChecked);
        string BarcodeGenerate();
    }
}
