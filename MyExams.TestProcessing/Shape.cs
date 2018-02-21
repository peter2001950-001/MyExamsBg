using AForge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.TestProcessing
{
   public class Shape
    {
      public Shape(IntPoint TopLeftPoint, IntPoint TopRightPoint, IntPoint BottomLeftPoint, IntPoint BottomRightPoint)
        {
            this.TopLeftPoint = TopLeftPoint;
            this.TopRightPoint = TopRightPoint;
            this.BottomLeftPoint = BottomLeftPoint;
            this.BottomRightPoint = BottomRightPoint;
        }
        public IntPoint TopLeftPoint { get; set; }
        public IntPoint TopRightPoint { get; set; }
        public IntPoint BottomLeftPoint { get; set; }
        public IntPoint BottomRightPoint { get; set; }
        public bool IsChecked { get; set; }
    }
}
