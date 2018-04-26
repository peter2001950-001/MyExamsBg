using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Services.Contracts
{
    public interface IUploadSessionService
    {
        IEnumerable<UploadSession> GetAll();
        void AddUploadSession(UploadSession uploadSession);
        IEnumerable<UploadSessionFileDirectory> GetUploadSessionFileDirectoriesBy(UploadSession uploadSession);
        IEnumerable<UploadSessionFileDirectory> GetAllUploadSessionFileDirectory();
        void AddUploadSessionFileDirectory(UploadSessionFileDirectory uploadSessionFileDirectory);
        UploadSession GetUploadSessionBy(FileDirectory fileDirectory);
        void ClearCache();
    }
}
