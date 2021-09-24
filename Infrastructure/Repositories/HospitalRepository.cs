using Infrastructure.Interfaces;
using Infrastructure.Interfaces.Implements;
using Infrastructure.Models;

namespace Infrastructure.Repositories
{
    public interface IHospitalRepository : IRepository<Hospital, int>
    {

    }
    public class HospitalRepository : Repository<Hospital, int>, IHospitalRepository
    {
        public HospitalRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }
    }
}
