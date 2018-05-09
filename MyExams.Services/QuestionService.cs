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
        private readonly IAnswerRepository _answerRepository;
        public QuestionService(IQuestionRepository questionRepository, ISectionRepository sectionRepository, IAnswerRepository answerRepository)
        {
            _questionRepository = questionRepository;
            _sectionRepository = sectionRepository;
            _answerRepository = answerRepository;
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
            var section = _sectionRepository.Where(x => x.Test.Id == testId).FirstOrDefault(c=>c.OrderNo == sectionNo);
            if (section != null)
            {

                return _questionRepository.Where(x => x.Section.Id == section.Id&&x.Active).OrderBy(x=>x.OrderNo);

            }
            return null;
        }
        public Question GetById(int id)
        {
            var question = _questionRepository.Where(x=>x.Id==id).FirstOrDefault();
            return question;
        }
        public IEnumerable<Question> GetAllByIds(List<int> ids)
        {
            return _questionRepository.Where(x => ids.Contains(x.Id));
        }

        public int GetPoints(int id)
        {
            return _questionRepository.GetPoints(id);
        }
        public Question QuestionHasChanged(ParseQuestionClass questionClass, Question question)
        {
            if(question.Text != questionClass.question.text||question.Points !=questionClass.question.points||question.MixupAnswers != questionClass.question.mixupOptions)
            {
                if (!question.IsInUse)
                {
                    question.Text = questionClass.question.text;
                    question.Points = questionClass.question.points;
                    question.MixupAnswers = questionClass.question.mixupOptions;

                    if (question.QuestionType == QuestionType.Text)
                    {
                        string[] sizes = { "Кратък", "Среден", "Дълъг" };
                        var index = Array.IndexOf(sizes, questionClass.question.selectedAnswerSize);
                        question.QuestionAnswerSize = (QuestionAnswerSize)index;
                        question.CorrectAnswer = questionClass.question.correctAnswer;
                    }
                    _questionRepository.SaveChanges();
                }
                else
                {
                    var newQuestion = new Question()
                    {
                        Active = true,
                        Text = questionClass.question.text,
                        QuestionType = question.QuestionType,
                        IsInUse = false,
                        OrderNo = question.OrderNo,
                        Section = question.Section,
                        Points = questionClass.question.points

                    };
                    if (question.QuestionType == QuestionType.Text)
                    {
                        newQuestion.MixupAnswers = false;
                        string[] sizes = { "Кратък", "Среден", "Дълъг" };
                        var index = Array.IndexOf(sizes, questionClass.question.selectedAnswerSize);
                        newQuestion.QuestionAnswerSize = (QuestionAnswerSize)index;
                        newQuestion.CorrectAnswer = questionClass.question.correctAnswer;
                    }
                    _questionRepository.Add(newQuestion);
                    _questionRepository.SaveChanges();
                    if (question.QuestionType == QuestionType.Choice)
                    {
                        var allAnswers = _answerRepository.Where(x => x.Question.Id == question.Id).ToList();
                        foreach (var item in allAnswers)
                        {
                            item.Question = newQuestion;
                        }
                    }
                    question.Active = false;
                    _questionRepository.SaveChanges();
                    return newQuestion;
                }
                
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
            if (question.IsInUse)
            {
                question.Active = false;
            }
            else
            {
                _questionRepository.Remove(question);
            }
            _questionRepository.SaveChanges();
        }

       
    }
}
