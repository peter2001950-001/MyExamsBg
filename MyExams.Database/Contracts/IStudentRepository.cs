﻿using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Database.Contracts
{
   public interface IStudentRepository :IRepositoryBase<Student>
    {
        Student GetStudentByUserId(string userId);
    }
}
