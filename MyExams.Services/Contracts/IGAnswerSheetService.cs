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
        GAnswerSheet GetGAnswerSheetBy(string barcode);
        IEnumerable<GAnswerSheet> GetAllGAnswerSheet();
        IEnumerable<GAnswerSheet> GetGAnswerSheetsBy(int GTestId);
        IEnumerable<GWrittenQuestion> GetAllGWrittenQuestions();
        IEnumerable<GQuestionToBeChecked> GetAllGQuestionToBeChecked();
        IEnumerable<GQuestionToBeChecked> GetAllGQuestionToBeCheckedBy(int teacherId);
        GAnswerSheet GetGAnswerSheetBy(int teacherId, int questionId);
        IEnumerable<GWrittenQuestion> GetGWrittenQuestionsBy(int GTestId);
        void AddGAnswerSheet(GAnswerSheet gAnswerSheet);
        void AddGWrittenQuestion(GWrittenQuestion gWrittenQuestion);
        void AddGQuestionToBeChecked(GQuestionToBeChecked gQuestionToBeChecked);
        void RemoveGQuestionToBeChecked(GQuestionToBeChecked gQuestionToBeChecked);
        string BarcodeGenerate();
        void ClearAnswerSheetCache();
        void ClearGWrittenQuestionCache();
        void ClearGQuestionsToBeCheckedCache();
    }
}
