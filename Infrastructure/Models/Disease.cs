using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Models
{
    public partial class Disease
    {
        public Disease()
        {
            HealthCheckDiseases = new HashSet<HealthCheckDisease>();
        }

        public int Id { get; set; }
        public string DiseaseCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DiseaseGroupId { get; set; }

        public bool? IsActive { get; set; }

        public virtual DiseaseGroup DiseaseGroup { get; set; }
        public virtual ICollection<HealthCheckDisease> HealthCheckDiseases { get; set; }
    }
}
