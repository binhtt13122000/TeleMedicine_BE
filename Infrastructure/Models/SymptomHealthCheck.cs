using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Models
{
    public partial class SymptomHealthCheck
    {
        public int Id { get; set; }
        public int SymptomId { get; set; }
        public int HealthCheckId { get; set; }
        public string Evidence { get; set; }

        public bool? IsActive { get; set; }

        public virtual HealthCheck HealthCheck { get; set; }
        public virtual Symptom Symptom { get; set; }
    }
}
