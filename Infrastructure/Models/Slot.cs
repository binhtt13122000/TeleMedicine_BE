using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Models
{
    public partial class Slot
    {
        public int Id { get; set; }
        public DateTime AssignedDate { get; set; }
        public int DoctorId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int? HealthCheckId { get; set; }

        public virtual Doctor Doctor { get; set; }
        public virtual HealthCheck HealthCheck { get; set; }
    }
}
