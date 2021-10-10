using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Models
{
    public partial class Patient
    {
        public Patient()
        {
            HealthChecks = new HashSet<HealthCheck>();
        }

        public int Id { get; set; }
        public string Email { get; set; }

        public string Name { get; set; }

        public string Avatar { get; set; }
        public string BackgroundDisease { get; set; }
        public string Allergy { get; set; }
        public string BloodGroup { get; set; }

        public bool? IsActive { get; set; }

        public virtual ICollection<HealthCheck> HealthChecks { get; set; }
    }
}
