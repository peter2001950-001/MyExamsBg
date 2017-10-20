﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
         public DateTime DateOfBirth { get; set; }
    }
}