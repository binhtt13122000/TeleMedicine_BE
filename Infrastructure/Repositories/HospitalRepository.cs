using Infrastructure.Interfaces;
using Infrastructure.Interfaces.Implements;
using Infrastructure.Models;
using System;
using System.Linq;

namespace Infrastructure.Repositories
{
    public interface IHospitalRepository : IRepository<Hospital, int>
    {
        bool IsDuplicatedHospitalCode(string hospitalCode);

        bool IsExistedHospitalId(int id);

        void AddRangeHospitalDoctor(HospitalDoctor[] hospitalDoctors);
    }
    public class HospitalRepository : Repository<Hospital, int>, IHospitalRepository
    {
        public HospitalRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }

        public void AddRangeHospitalDoctor(HospitalDoctor[] entity)
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

        public bool IsDuplicatedHospitalCode(string hospitalCode)
        {
            Hospital checkHospitalExisted = GetAll().Where(s => hospitalCode.ToUpper().Equals(s.HospitalCode.ToUpper())).FirstOrDefault();
            return checkHospitalExisted != null;
        }

        public bool IsExistedHospitalId(int id)
        {
            Hospital checkHospitalExisted = GetAll().Where(s => s.Id == id).FirstOrDefault();
            return checkHospitalExisted != null;
        }
    }
}
