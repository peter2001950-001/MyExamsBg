﻿using MyExams.Database.Contracts;
using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Database.Repositories
{
    public class GAnswerRepository : RepositoryBase<GAnswer>, IGAnswerRepository
    {
        public GAnswerRepository(IDatabase database) : base(database)
        {
        }
    }
}