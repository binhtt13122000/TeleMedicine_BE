using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Models
{
    public partial class Hospital
    {
        public Hospital()
        {
            HospitalDoctors = new HashSet<HospitalDoctor>();
        }

        public int Id { get; set; }
        public string HospitalCode { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }

        public double Lat { get; set; }

        public double Long { get; set; }

        public bool? IsActive { get; set; }

        public virtual ICollection<HospitalDoctor> HospitalDoctors { get; set; }
    }
}
