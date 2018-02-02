﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Models
{
    public class GWrittenQuestion
    {
        [Key]
        public int Id { get; set; }
        public GQuestion GQuestion { get; set; }
        public bool IsChecked { get; set; }
        public int ReceivedPoints { get; set; }
        public string FileName { get; set; }

    }
}
