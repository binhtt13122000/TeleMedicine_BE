using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ViewModels
{
    public class PrescriptionHealthCheckVM
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

        public bool? IsActive { get; set; }

        public virtual DrugVM Drug { get; set; }

    }

    public class PrescriptionCM
    {
        public int HealthCheckId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DrugId { get; set; }
        public int? MorningQuantity { get; set; }
        public int? AfternoonQuantity { get; set; }
        public int? EveningQuantity { get; set; }
        public string Description { get; set; }

    }

    public class PrescriptionHealthCheckCM
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DrugId { get; set; }
        public int? MorningQuantity { get; set; }
        public int? AfternoonQuantity { get; set; }
        public int? EveningQuantity { get; set; }
        public string Description { get; set; }
    }
}
