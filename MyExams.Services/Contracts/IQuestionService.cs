﻿using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Services.Contracts
{
    public interface IQuestionService
    {
         void AddQuestion(Question question);
         IEnumerable<Question> GetAll();
         IEnumerable<Question> GetAllQuestionsBy(int testId, int sectionNo);
    }
}
