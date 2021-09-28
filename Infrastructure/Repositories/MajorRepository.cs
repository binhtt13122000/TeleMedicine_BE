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
    public interface IMajorRepository : IRepository<Major, int>
    {
        bool IsExistedMajorId(int Id);

        void AddRangeMajorDoctor(MajorDoctor[] majorDoctors);
    }
    public class MajorRepository : Repository<Major, int>, IMajorRepository
    {
        public MajorRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }

        public void AddRangeMajorDoctor(MajorDoctor[] entity)
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

        public bool IsExistedMajorId(int Id)
        {
            Major checkExistedMajor = GetAll().Where(s => s.Id == Id).FirstOrDefault();
            return checkExistedMajor != null;
        }
    }

}
