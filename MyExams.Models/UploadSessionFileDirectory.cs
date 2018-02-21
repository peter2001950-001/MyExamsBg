using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Models
{
    /// <summary>
    /// It connects the upload session with all the file directories
    /// </summary>
   public class UploadSessionFileDirectory
    {
        [Key]
        public int Id { get; set; }
        public UploadSession UploadSession { get; set; }
        public FileDirectory FileDirectory { get; set; }
        public UploadedFileStatus UploadedFileStatus { get; set; }
    }
    public enum UploadedFileStatus
    {
        /// <summary>
        /// When the file has not been processed yet
        /// </summary>
        NotChecked,
        /// <summary>
        ///  When the file has been processed successfully
        /// </summary>
        Checked,
        /// <summary>
        /// When the file has been processed before
        /// </summary>
        AlreadyChecked,
        /// <summary>
        /// When there is an error 
        /// </summary>
        HaveProblem,
        /// <summary>
        /// When the file has not been process because the barcode has not been found
        /// </summary>
        FileNotRecognised
    }
}
