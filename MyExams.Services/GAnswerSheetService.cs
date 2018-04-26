using MyExams.Database.Contracts;
using MyExams.Models;
using MyExams.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Services
{
    public class GAnswerSheetService:IGAnswerSheetService
    {
        private readonly IGAnswerSheetRepository _gAnswerSheetRepository;
        private readonly IGWrittenQuestionRepository _gWrittenQuestionRepository;
        private readonly IGQuestionsToBeCheckedRepository _gQuestionsToBeCheckedRepository;

        public GAnswerSheetService(IGAnswerSheetRepository gAnswerSheetRepository, IGWrittenQuestionRepository gWrittenQuestionRepository, IGQuestionsToBeCheckedRepository gQuestionsToBeCheckedRepository)
        {
            _gAnswerSheetRepository = gAnswerSheetRepository;
            _gWrittenQuestionRepository = gWrittenQuestionRepository;
            _gQuestionsToBeCheckedRepository = gQuestionsToBeCheckedRepository;
        }
        public IEnumerable<GAnswerSheet> GetAllGAnswerSheet()
        {
            return _gAnswerSheetRepository.GetAll();
        }
        public IEnumerable<GAnswerSheet> GetGAnswerSheetsBy(int GTestId)
        {
          return  _gAnswerSheetRepository.Include(x => x.GTest).Where(x => x.GTest.Id == GTestId);
        }
        public IEnumerable<GWrittenQuestion> GetAllGWrittenQuestions()
        {
            return _gWrittenQuestionRepository.GetAll();
        }
        public GWrittenQuestion GetGWrittenQuestionsBy(int GTestId, int orderNo)
        {
            return _gWrittenQuestionRepository.GetWrittenQuestionBy(GTestId, orderNo);
        }
        public IEnumerable<GWrittenQuestion> GetGWrittenQuestionsBy(int GTestId)
        {
            return _gWrittenQuestionRepository.GetWrittenQuestionBy(GTestId);
        }
        public IEnumerable<GQuestionToBeChecked> GetAllGQuestionToBeChecked()
        {
            return _gQuestionsToBeCheckedRepository.GetAll();
        }
        public IEnumerable<GQuestionToBeChecked> GetAllGQuestionToBeCheckedBy(int teacherId)
        {
            return _gQuestionsToBeCheckedRepository.GetQuestionsToBeCheckedBy(teacherId);
        }
        public GAnswerSheet GetGAnswerSheetBy(int teacherId, int questionId)
        {
            var question = this.GetAllGQuestionToBeCheckedBy(teacherId).Where(x => x.Id == questionId).FirstOrDefault();
            var answerSheets = GetGAnswerSheetsBy(question.GWrittenQuestion.GTest.Id).ToList();
            var selectedAS =  answerSheets.Where(x => x.FirstQuestionNo <= question.GWrittenQuestion.GQuestionId && x.LastQuestionNo >= question.GWrittenQuestion.GQuestionId).FirstOrDefault();

            return selectedAS;
        }
        public GAnswerSheet GetGAnswerSheetBy(string barcode)
        {
            var answerSheet = _gAnswerSheetRepository.WhereIncludeAll(x => x.Barcode == barcode).FirstOrDefault();
            return answerSheet;
        }
        public void AddGAnswerSheet(GAnswerSheet gAnswerSheet)
        {
            _gAnswerSheetRepository.Add(gAnswerSheet);
            _gAnswerSheetRepository.SaveChanges();
        }
        public void AddGWrittenQuestion(GWrittenQuestion gWrittenQuestion)
        {
            _gWrittenQuestionRepository.Add(gWrittenQuestion);
            _gWrittenQuestionRepository.SaveChanges();
        }
        public void AddGQuestionToBeChecked(GQuestionToBeChecked gQuestionToBeChecked)
        {
            _gQuestionsToBeCheckedRepository.Add(gQuestionToBeChecked);
            _gQuestionsToBeCheckedRepository.SaveChanges();
        }
        public void RemoveGQuestionToBeChecked(GQuestionToBeChecked gQuestionToBeChecked)
        {
            try
            {
                    _gQuestionsToBeCheckedRepository.Remove(gQuestionToBeChecked);
            }
            catch (Exception)
            {
                var newQuery = _gQuestionsToBeCheckedRepository.Where(x => x.Id == gQuestionToBeChecked.Id).FirstOrDefault();
                _gQuestionsToBeCheckedRepository.Remove(newQuery);
            }
           
            _gQuestionsToBeCheckedRepository.SaveChanges();
        }

        public string BarcodeGenerate()
        {
            string result = "";
            var random = new Random();
            for (int i = 0; i < 16; i++)
            {
                result += random.Next(0, 9);
            }
            if(_gAnswerSheetRepository.GetAll().Any(x=>x.Barcode == result))
            {
                return BarcodeGenerate();
            }
            return result;
        }

        public void ClearAnswerSheetCache()
        {
            _gAnswerSheetRepository.ClearCache();
        }

        public void ClearGWrittenQuestionCache()
        {
            _gWrittenQuestionRepository.ClearCache();
        }

        public void ClearGQuestionsToBeCheckedCache()
        {
            _gQuestionsToBeCheckedRepository.ClearCache();
        }

       
    }
}
