using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ViewModels
{
    public class HealthCheckDiseaseVM
    {
        public int Id { get; set; }
        public int HealthCheckId { get; set; }
        public int DiseaseId { get; set; }

        public virtual DiseaseVM Disease { get; set; }
    }

    public class HealthCheckDiseaseCM
    {
        public int DiseaseId { get; set; }

    }
}
