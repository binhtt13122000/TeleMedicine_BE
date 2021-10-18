using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ViewModels
{
    public enum HealthCheckFieldEnum
    {
        Id,
        Height,
        Weight,
        ReasonCancel,
        Rating,
        Advice,
        PatientId,
        CreatedTime,
        CanceledTime
    }

    public enum TypeSearch
    {
        NORMAL,
        NEAREST,
    }

    public enum HealthCheckStatus
    {
        ALL,
        BOOKED,
        CANCELED,
        COMPLETED,
    }

    public enum HealthCheckMode
    {
        NORMAL,
        CALL
    }

    public enum HealthCheckTypeRole
    {
        DOCTORS,
        USERS
    }

    public class HealthCheckStatusUM
    {
        public int Id;
        public string ReasonCancel { get; set; }

        public HealthCheckSta status;

    }

    public class HealthCheckVM
    {
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

        public virtual PatientHealthCheckVM Patient { get; set; }

        public virtual ICollection<HealthCheckDiseaseVM> HealthCheckDiseases { get; set; }
        public virtual ICollection<PrescriptionHealthCheckVM> Prescriptions { get; set; }
        public virtual ICollection<SlotHealthCheckVM> Slots { get; set; }
        public virtual ICollection<SymptomHealthCheckVM> SymptomHealthChecks { get; set; }
    }

    public class HealthCheckCM
    {
        public int? Height { get; set; }
        public int? Weight { get; set; }
        public string Token { get; set; }

        [Required]
        public int PatientId { get; set; }

        
        [Required]
        public int SlotId { get; set; }


        public virtual ICollection<SymptomHealthCheckCM> SymptomHealthChecks { get; set; }
    }

    public class HealthCheckUM
    {
        [Required]
        public int Id;

        public int Rating { get; set; }

        public string Comment { get; set; }

        public string Advice { get; set; }

        public virtual ICollection<HealthCheckDiseaseCM> HealthCheckDiseases { get; set; }
        public virtual ICollection<PrescriptionCM> Prescriptions { get; set; }
        public virtual ICollection<SymptomHealthCheckCM> SymptomHealthChecks { get; set; }
    }
}
