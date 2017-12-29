using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Services.Contracts
{
   public interface ISectionService
    {
         IEnumerable<Section> GetAll();
        IEnumerable<Section> GetAllSectionsByTestId(int testId);
        void AddSection(Section section);
    }
}
