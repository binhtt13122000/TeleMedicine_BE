using Infrastructure.Interfaces;
using Infrastructure.Interfaces.Implements;
using Infrastructure.Models;
using System.Linq;

namespace Infrastructure.Repositories
{
    public interface IHospitalRepository : IRepository<Hospital, int>
    {
        bool IsDuplicatedHospitalCode(string hospitalCode);
    }
    public class HospitalRepository : Repository<Hospital, int>, IHospitalRepository
    {
        public HospitalRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }

        public bool IsDuplicatedHospitalCode(string hospitalCode)
        {
            Hospital checkHospitalExisted = GetAll().Where(s => hospitalCode.ToUpper().Equals(s.HospitalCode.ToUpper())).FirstOrDefault();
            return checkHospitalExisted != null;
        }
    }
}
