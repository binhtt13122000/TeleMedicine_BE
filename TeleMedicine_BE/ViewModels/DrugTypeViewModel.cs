using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ViewModels
{

    public enum DrugTypeFieldEnum
    {
        Id,
        Name,
    }
    public class DrugTypeVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public bool? IsActive { get; set; }
    }

    public class DrugTypeUM
    {
        [Required]
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string Name { get; set; }
        [StringLength(256)]
        public string Description { get; set; }

        [Required]
        public bool? IsActive { get; set; }
    }

    public class DrugTypeCM
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string Name { get; set; }
        [StringLength(256)]
        public string Description { get; set; }
    }
}
