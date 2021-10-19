using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Models
{
    public partial class Doctor
    {
        public Doctor()
        {
            CertificationDoctors = new HashSet<CertificationDoctor>();
            HospitalDoctors = new HashSet<HospitalDoctor>();
            MajorDoctors = new HashSet<MajorDoctor>();
            Slots = new HashSet<Slot>();
        }

        public int Id { get; set; }
        public string Email { get; set; }

        public string Name { get; set; }
        
        public string Avatar { get; set; }
        public string PractisingCertificate { get; set; }
        public string CertificateCode { get; set; }
        public string PlaceOfCertificate { get; set; }
        public DateTime DateOfCertificate { get; set; }
        public string ScopeOfPractice { get; set; }
        public string Description { get; set; }
        public int? NumberOfConsultants { get; set; }

        public int? NumberOfCancels { get; set; }
        public int? Rating { get; set; }
        public bool? IsVerify { get; set; }

        public bool? IsActive { get; set; }

        public virtual ICollection<CertificationDoctor> CertificationDoctors { get; set; }
        public virtual ICollection<HospitalDoctor> HospitalDoctors { get; set; }
        public virtual ICollection<MajorDoctor> MajorDoctors { get; set; }
        public virtual ICollection<Slot> Slots { get; set; }
    }
}
