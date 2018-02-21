using AForge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.TestProcessing
{
   public class Line
    {
        public IntPoint APoint { get; set; }
        public IntPoint BPoint { get; set; }
        public int Height

        {
            get
            {
                return BPoint.Y - APoint.Y;
            }

        }
        public int Width { get { return BPoint.X - APoint.X; } }
    }
}
