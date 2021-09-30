using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ViewModels
{
    public enum PatientFieldEnum
    {
        Id,
        Email,
        BackgroundDisease,
        Allergy,
        BloodGroup
    }
    public class PatientVM
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string BackgroundDisease { get; set; }

        public string Allergy { get; set; }

        public string BloodGroup { get; set; }

        public virtual ICollection<HealthCheck> HealthChecks { get; set; }

    }

    public class PatientCM
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string Email { get; set; }

        public string BackgroundDisease { get; set; }

        public string Allergy { get; set; }

        public string BloodGroup { get; set; }
    }

    public class PatientUM
    {
        [Required]
        public int Id;

        public string BackgroundDisease { get; set; }

        public string Allergy { get; set; }

        public string BloodGroup { get; set; }
    }
}
