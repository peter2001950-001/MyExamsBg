using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyExams.Models
{

 
    public class NewClassViewModel
    {
        [Display(Name ="Име на класа")]
        public string Name { get; set; }
        [Display(Name ="Учебен предмет")]
        public string Subject { get; set; }
    }
    public class JoinClassViewModel
    {
        [Display(Name = "Уникален код на класа")]
        public string ClassCode { get; set; }
        public string noInClass { get; set; }
    }

    public class DisplayClassTeacherViewModel
    {
        public string ClassName { get; set; }
        public string Subject { get; set; }
        public bool FirstMessageShowed { get; set; }
        public int  StudentsCount { get; set; }
        public double AverageMark { get; set; }
        public string UniqueCode { get; set; }
        public string  ClassColor { get; set; }
    }
    public class DisplayClassStudentViewModel
    {
        public string ClassName { get; set; }
        public string Subject { get; set; }
        public int StudentsCount { get; set; }
        public string UniqueCode { get; set; }
        public string ClassColor { get; set; }
        public string TeacherName { get; set; }
    }
}