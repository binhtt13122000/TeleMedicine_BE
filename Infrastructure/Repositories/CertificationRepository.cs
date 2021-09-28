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
        bool IsExistedCertificationId(int Id);
        void AddRangeCertification(CertificationDoctor[] certificationDoctor);
    }
    public class CertificationRepository : Repository<Certification, int>, ICertificationRepository
    {
        public CertificationRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }

        public bool IsExistedCertificationId(int Id)
        {
            Certification checkExistedCertifiId = GetAll().Where(s => s.Id == Id).FirstOrDefault();
            return checkExistedCertifiId != null;
        }

        public void AddRangeCertification(CertificationDoctor[] entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }
            try
            {
                _dbContext.AddRangeAsync(entity);
                _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be saved: {ex.Message}");
            }
        }
    }
}
