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
   public  class GAnswerService:IGAnswerService
    {
        private readonly IGAnswerRepository _gAnswerRepository;
        public GAnswerService(IGAnswerRepository gAnswerRepository)
        {
            _gAnswerRepository = gAnswerRepository;
        }
        public IEnumerable<GAnswer> GetAll()
        {
            return _gAnswerRepository.GetAll();
        }
        public IEnumerable<GAnswer> GetAllBy(int answerId)
        {
            return _gAnswerRepository.Where(x => x.Answer.Id == answerId);
        }
        public IEnumerable<GAnswer> GetAllBy(List<int> answerIds)
        {
            return _gAnswerRepository.GetAllBy(answerIds);
        }
    }
}
