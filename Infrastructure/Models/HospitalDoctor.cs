using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Models
{
    public partial class HospitalDoctor
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int HospitalId { get; set; }
        public bool IsWorking { get; set; }

        public bool? IsActive { get; set; }

        public virtual Doctor Doctor { get; set; }
        public virtual Hospital Hospital { get; set; }
    }
}
