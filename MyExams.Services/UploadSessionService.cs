using MyExams.Database.Contracts;
using MyExams.Models;
using MyExams.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Services
{
    public class UploadSessionService:IUploadSessionService
    {
        private readonly IUploadSessionRepository _uploadSessionRepository;
        private readonly IUploadSessionFileDirectoryRepository _uploadSessionFileDirectoryRepository;
        public UploadSessionService(IUploadSessionRepository uploadSessionRepository, IUploadSessionFileDirectoryRepository uploadSessionFileDirectoryRepository)
        {
            if (_uploadSessionRepository == null) _uploadSessionRepository = uploadSessionRepository;
            if (_uploadSessionFileDirectoryRepository == null) _uploadSessionFileDirectoryRepository = uploadSessionFileDirectoryRepository;
        }
        public IEnumerable<UploadSession> GetAll()
        {
            return _uploadSessionRepository.GetAll();
        }
        public IEnumerable<UploadSessionFileDirectory> GetAllUploadSessionFileDirectory()
        {
            return _uploadSessionFileDirectoryRepository.GetAll();
        }
        public UploadSession GetUploadSessionBy(FileDirectory fileDirectory)
        {
           var usfd =  _uploadSessionFileDirectoryRepository.Where(x => x.FileDirectory.Id == fileDirectory.Id).FirstOrDefault();
           return _uploadSessionRepository.GetAll().Where(x => x.Id == usfd.UploadSession.Id).FirstOrDefault();
        }
        public void AddUploadSessionFileDirectory(UploadSessionFileDirectory uploadSessionFileDirectory)
        {
            _uploadSessionFileDirectoryRepository.Add(uploadSessionFileDirectory);
            _uploadSessionFileDirectoryRepository.SaveChanges();
        }
        public IEnumerable<UploadSessionFileDirectory> GetUploadSessionFileDirectoriesBy(UploadSession uploadSession)
        {
            return _uploadSessionFileDirectoryRepository.Where(x => x.UploadSession.Id == uploadSession.Id);
        }
        public void AddUploadSession(UploadSession uploadSession)
        {
            _uploadSessionRepository.Add(uploadSession);
            _uploadSessionRepository.SaveChanges();
        }
        
    }
}
