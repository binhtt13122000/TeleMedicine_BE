using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Models
{
    public partial class Symptom
    {
        public Symptom()
        {
            SymptomHealthChecks = new HashSet<SymptomHealthCheck>();
        }

        public int Id { get; set; }
        public string SymptomCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public bool? IsActive { get; set; }

        public virtual ICollection<SymptomHealthCheck> SymptomHealthChecks { get; set; }
    }
}
