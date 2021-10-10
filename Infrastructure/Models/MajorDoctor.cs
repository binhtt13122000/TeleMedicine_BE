using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Models
{
    public partial class MajorDoctor
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int MajorId { get; set; }

        public bool? IsActive { get; set; }

        public virtual Doctor Doctor { get; set; }
        public virtual Major Major { get; set; }
    }
}
