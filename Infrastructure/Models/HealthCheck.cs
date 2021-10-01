using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Models
{
    public enum HealthCheckSta
    {
        BOOKED,
        CANCELED,
        COMPLETED,
    }
    public partial class HealthCheck
    {

        public HealthCheck()
        {
            HealthCheckDiseases = new HashSet<HealthCheckDisease>();
            Prescriptions = new HashSet<Prescription>();
            Slots = new HashSet<Slot>();
            SymptomHealthChecks = new HashSet<SymptomHealthCheck>();
        }

        public int Id { get; set; }
        public int? Height { get; set; }
        public int? Weight { get; set; }
        public string ReasonCancel { get; set; }
        public int? Rating { get; set; }
        public string Comment { get; set; }
        public string Advice { get; set; }
        public string Token { get; set; }
        public int PatientId { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? CanceledTime { get; set; }

        public string Status { get; set; }

        public virtual Patient Patient { get; set; }
        public virtual ICollection<HealthCheckDisease> HealthCheckDiseases { get; set; }
        public virtual ICollection<Prescription> Prescriptions { get; set; }
        public virtual ICollection<Slot> Slots { get; set; }
        public virtual ICollection<SymptomHealthCheck> SymptomHealthChecks { get; set; }
    }
}
