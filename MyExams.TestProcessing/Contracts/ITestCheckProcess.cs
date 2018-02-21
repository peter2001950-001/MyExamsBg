using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.TestProcessing.Contracts
{
   public interface ITestCheckProcess
    {
        void SetBitmap(Bitmap bitmap);
        void SetSaveFileName(string path);
        void SetBitmapFileDirectory(FileDirectory file);
        GTest StartChecking();
    }
}
