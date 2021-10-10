using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Models
{
    public partial class Drug
    {
        public Drug()
        {
            Prescriptions = new HashSet<Prescription>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Producer { get; set; }
        public string DrugOrigin { get; set; }
        public string DrugForm { get; set; }
        public int DrugTypeId { get; set; }

        public bool? IsActive { get; set; }

        public virtual DrugType DrugType { get; set; }
        public virtual ICollection<Prescription> Prescriptions { get; set; }
    }
}
