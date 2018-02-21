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
    public class FileDirectoryService :IFileDirectoryService
    {
        private readonly IFileDirectoryRepository _fileDirectoryRepository;
        private readonly IUploadSessionFileDirectoryRepository _uploadSessionFileDirectoryRepository;
        public FileDirectoryService(IFileDirectoryRepository fileDirectoryRepository, IUploadSessionFileDirectoryRepository uploadSessionFileDirectoryRepository)
        {
            if (_fileDirectoryRepository == null) _fileDirectoryRepository = fileDirectoryRepository;
            if (_uploadSessionFileDirectoryRepository == null) _uploadSessionFileDirectoryRepository = uploadSessionFileDirectoryRepository;
        }
        

        public IEnumerable<FileDirectory> GetAll()
        {
            return _fileDirectoryRepository.GetAll();
        }
       
        public void AddFileDirectory(FileDirectory fileDirectory)
        {
            _fileDirectoryRepository.Add(fileDirectory);
            _fileDirectoryRepository.SaveChanges();
        }
    }
}
