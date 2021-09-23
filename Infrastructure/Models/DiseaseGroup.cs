using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Models
{
    public partial class DiseaseGroup
    {
        public DiseaseGroup()
        {
            Diseases = new HashSet<Disease>();
        }

        public int Id { get; set; }
        public string GroupName { get; set; }

        public virtual ICollection<Disease> Diseases { get; set; }
    }
}
