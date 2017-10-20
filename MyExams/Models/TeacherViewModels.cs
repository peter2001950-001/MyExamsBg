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

    public class DisplayClassViewModel
    {
        public string ClassName { get; set; }
        public string Subject { get; set; }
    }
}