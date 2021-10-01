using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ViewModels
{
    public class SymptomHealthCheckCM
    {

        [Required]
        public int SymptomId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(256)]
        public string Evidence { get; set; }

    }

    public partial class SymptomHealthCheckVM
    {
        public int Id { get; set; }
        public int SymptomId { get; set; }
        public int HealthCheckId { get; set; }
        public string Evidence { get; set; }

        public virtual SymptomVM Symptom { get; set; }
    }
}
