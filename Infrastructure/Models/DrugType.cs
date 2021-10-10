using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Models
{
    public partial class DrugType
    {
        public DrugType()
        {
            Drugs = new HashSet<Drug>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public bool? IsActive { get; set; }

        public virtual ICollection<Drug> Drugs { get; set; }
    }
}
