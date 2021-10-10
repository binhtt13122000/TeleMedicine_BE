using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Models
{
    public partial class HealthCheckDisease
    {
        public int Id { get; set; }
        public int HealthCheckId { get; set; }
        public int DiseaseId { get; set; }

        public bool? IsActive { get; set; }

        public virtual Disease Disease { get; set; }
        public virtual HealthCheck HealthCheck { get; set; }
    }
}
