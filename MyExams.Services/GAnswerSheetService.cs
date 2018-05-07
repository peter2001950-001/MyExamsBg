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
        public IEnumerable<GAnswerSheet> GetGAnswerSheetsBy(int gTestId)
        {
            return _gAnswerSheetRepository.GetGAnswerSheetsBy(gTestId);
        }
        public IEnumerable<GWrittenQuestion> GetAllGWrittenQuestions()
        {
            return _gWrittenQuestionRepository.GetAll();
        }
        public IEnumerable<GWrittenQuestion> GetGWrittenQuestionsBy(int gTestId)
        {
            return _gWrittenQuestionRepository.GetWrittenQuestionsBy(gTestId);
        }
        public IEnumerable<GQuestionToBeChecked> GetAllGQuestionToBeChecked()
        {
            return _gQuestionsToBeCheckedRepository.GetAll();
        }
        public IEnumerable<GQuestionToBeChecked> GetAllGQuestionToBeCheckedBy(int teacherId)
        {
            return _gQuestionsToBeCheckedRepository.Where(x => x.Teacher.Id == teacherId);
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
            _gQuestionsToBeCheckedRepository.Remove(gQuestionToBeChecked);
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
    }
}
