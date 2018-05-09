
using MyExams.Database.Contracts;
using MyExams.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExams.Database.Repositories
{
    public class UploadSessionRepository : RepositoryBase<UploadSession>, IUploadSessionRepository
    {
        public UploadSessionRepository(IDatabase database) : base(database)
        {
        }
        public override IEnumerable<UploadSession> GetAll()
        {
            return _dbSet.AsNoTracking().AsEnumerable();
        }
        public override void Add(UploadSession item)
        {
            if(item.SessionIdentifier == null)
            {
                var random = new Random();
                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

                item.SessionIdentifier = new string(Enumerable.Repeat(chars, 32)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            }
            base.Add(item);
        }
        public override void ClearCache()
        {
            
        }
    }
}
