using Infrastructure.Interfaces;
using Infrastructure.Interfaces.Implements;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface ICertificationRepository : IRepository<Certification, int>
    {

    }
    public class CertificationRepository : Repository<Certification, int>, ICertificationRepository
    {
        public CertificationRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }
    }
}
