using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Models
{
    public partial class Certification
    {
        public Certification()
        {
            CertificationDoctors = new HashSet<CertificationDoctor>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public bool? IsActive { get; set; }

        public virtual ICollection<CertificationDoctor> CertificationDoctors { get; set; }
    }
}
