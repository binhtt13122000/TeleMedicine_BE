using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Models
{
    public partial class Prescription
    {
        public int Id { get; set; }
        public int HealthCheckId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DrugId { get; set; }
        public int? MorningQuantity { get; set; }
        public int? AfternoonQuantity { get; set; }
        public int? EveningQuantity { get; set; }
        public string Description { get; set; }

        public virtual Drug Drug { get; set; }
        public virtual HealthCheck HealthCheck { get; set; }
    }
}
