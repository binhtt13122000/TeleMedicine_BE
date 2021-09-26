using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ViewModels
{
    public class DiseaseVM
    {
        public int Id { get; set; }
        public string DiseaseCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DiseaseGroupId { get; set; }

        public virtual DiseaseGroupVM DiseaseGroup { get; set; }
    }

    public class DiseaseUM
    {
        [Required]
        public int Id;

        [Required(AllowEmptyStrings = false)]
        public string DiseaseCode { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(256)]
        public string Description { get; set; }

        [Required]
        public int DiseaseGroupId { get; set; }
    }

    public class DiseaseCM
    {
        
        [Required(AllowEmptyStrings = false)]
        public string DiseaseCode { get; set; }
        
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(256)]
        public string Description { get; set; }
        
        [Required]
        public int DiseaseGroupId { get; set; }
    }
}
