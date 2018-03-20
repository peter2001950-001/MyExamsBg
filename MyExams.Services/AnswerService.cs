using MyExams.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyExams.Models;
using MyExams.Database.Contracts;

namespace MyExams.Services
{
    public class AnswerService : IAnswerService
    {

        private readonly IAnswerRepository _answerRepository;
        private readonly IQuestionService _questionService;
        public AnswerService(IAnswerRepository answerRepository, IQuestionService questionService)
        {
            _answerRepository = answerRepository;
            _questionService = questionService;
        }
        public void AddAnswer(Answer answer)
        {
            _answerRepository.Add(answer);
            _answerRepository.SaveChanges();
        }

        public IEnumerable<Answer> GetAll()
        {
            return _answerRepository.GetAll();
        }
        public IEnumerable<Answer> GetAllByQuestionIds(List<int> ids)
        {
            return _answerRepository.Where(x => ids.Contains(x.Question.Id));
        }
        public void Update(Answer answer)
        {
            _answerRepository.Update(answer);
        }

        public IEnumerable<Answer> GetAllBy(int testId, int sectionNo, int questionNo)
        {
            var questions = _questionService.GetAllQuestionsBy(testId, sectionNo);
            if (questions != null)
            {
                var question =  questions.Where(x => x.OrderNo == questionNo).FirstOrDefault();
                return  _answerRepository.GetAll().Where(x => x.Question.Id == question.Id);
            }
            return null;
        }
        public bool GetIsCorrect(int id)
        {
            return _answerRepository.GetIsCorrect(id);
        }
        public void RemoveAnswer(Answer answer)
        {
            var answers = _answerRepository.GetAll().Where(x => x.Question.Id == answer.Question.Id);
            foreach (var item in answers)
            {
                if (item.OrderNo > answer.OrderNo)
                {
                    item.OrderNo--;
                }
            }
            _answerRepository.Remove(answer);
            _answerRepository.SaveChanges();
        }
      
        
    }
}
