using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ViewModels
{
    public class DiseaseGroupVM
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
    }

    public class DiseaseGroupCM
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string GroupName { get; set; }
    }

    public class DiseaseGroupUM
    {
        [Required]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string GroupName { get; set; }
    }
}
