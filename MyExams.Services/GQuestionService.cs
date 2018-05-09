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
    public class GQuestionService: IGQuestionService
    {
        private readonly IGQuestionRepository _gQuestionRepository;
        public GQuestionService(IGQuestionRepository gQuestionRepository)
        {
            _gQuestionRepository = gQuestionRepository;
        }
        public IEnumerable<GQuestion> GetAllBy(int gTestId)
        {
           return _gQuestionRepository.WhereIncludeAll(x => x.GTest.Id == gTestId).ToList();
        }
        public IEnumerable<GQuestion>  GetAllBy(List<int> gTestIds)
        {
            var gQuestions = _gQuestionRepository.WhereIncludeAll(x => gTestIds.Contains(x.GTest.Id)).ToList();
            return gQuestions;
        }
        public void AddNewGQuestion(GQuestion gQuestion)
        {
            _gQuestionRepository.Add(gQuestion);
            _gQuestionRepository.SaveChanges();
        }
    }
}
