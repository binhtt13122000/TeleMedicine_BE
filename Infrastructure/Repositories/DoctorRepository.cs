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
    public interface IDoctorRepository : IRepository<Doctor, int>
    {
        bool isDuplicatedEmail(string email);

        bool isDuplicatedCertificationCode(string certificationCode);

    }
    public class DoctorRepository : Repository<Doctor, int>, IDoctorRepository
    {
        public DoctorRepository(TeleMedicineContext dbContext) : base(dbContext)
        {

        }

        public bool isDuplicatedCertificationCode(string certificationCode)
        {
            Doctor checkDoctorExisted = GetAll().Where(s => s.CertificateCode.ToUpper().Equals(certificationCode.Trim().ToUpper())).FirstOrDefault();
            return checkDoctorExisted != null;
        }

        public bool isDuplicatedEmail(string email)
        {
            Doctor checkDoctorExisted = GetAll().Where(s => s.Email.ToUpper().Equals(email.Trim().ToUpper())).FirstOrDefault();
            return checkDoctorExisted != null;
        }
    }
}
