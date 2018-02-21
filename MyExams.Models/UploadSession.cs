using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Models
{
    /// <summary>
    /// All files that have been uploaded at once are a session
    /// </summary>
   public class UploadSession
    {
        [Key]
        public int Id { get; set; }
        public Teacher Teacher { get; set; }
        /// <summary>
        /// The unique code used to check the status of the session
        /// </summary>
        public string SessionIdentifier { get; set; }
        /// <summary>
        /// Total files that have been uploaded
        /// </summary>
        public int TotalUploaded { get; set; }
        /// <summary>
        /// Total number of files that have been checked
        /// </summary>
        public int TotalFinished { get; set; }
        /// <summary>
        /// if all files have been processed
        /// </summary>
        public bool IsDone { get { return TotalFinished == TotalUploaded; } }
    }
}
