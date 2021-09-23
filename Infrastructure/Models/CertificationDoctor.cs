using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Models
{
    public partial class CertificationDoctor
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int CertificationId { get; set; }
        public string Evidence { get; set; }
        public DateTime DateOfIssue { get; set; }

        public virtual Certification Certification { get; set; }
        public virtual Doctor Doctor { get; set; }
    }
}
