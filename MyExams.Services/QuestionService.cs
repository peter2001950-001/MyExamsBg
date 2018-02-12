using MyExams.Database.Contracts;
using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Services.Contracts
{
   public class QuestionService:IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly ISectionRepository _sectionRepository;
        public QuestionService(IQuestionRepository questionRepository, ISectionRepository sectionRepository)
        {
            _questionRepository = questionRepository;
            _sectionRepository = sectionRepository;
        }
        public void AddQuestion(Question question)
        {
            _questionRepository.Add(question);
            _questionRepository.SaveChanges();
        }
        public void Update(Question question)
        {
            _questionRepository.Update(question);
        }
        public IEnumerable<Question> GetAll()
        {
            return _questionRepository.GetAll();
        }

        public IEnumerable<Question> GetAllQuestionsBy(int testId, int sectionNo)
        {
            var section = _sectionRepository.GetAll().Where(x => x.Test.Id == testId).FirstOrDefault(c=>c.OrderNo == sectionNo);
            if (section != null)
            {
                return _questionRepository.GetAll().Where(x => x.Section.Id == section.Id);
            }
            return null;
        }

        public void RemoveQuestion(Question question)
        {
            var questions = _questionRepository.GetAll().Where(x => x.Section.Id == question.Section.Id);
            foreach (var item in questions)
            {
                if (item.OrderNo > question.OrderNo)
                {
                    item.OrderNo--;
                }
            }
            _questionRepository.Remove(question);
            _questionRepository.SaveChanges();
        }

       
    }
}
