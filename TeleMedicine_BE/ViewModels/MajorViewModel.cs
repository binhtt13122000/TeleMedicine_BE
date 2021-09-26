using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ViewModels
{
    public class MajorVM
    {
        public int Id { get; set; }
        public String Name { get; set; }
        
        public String Description { get; set; }
    }
    
    public class MajorCM
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public String Name { get; set; }

        [StringLength(256)]
        public String Description { get; set; }
    }

    public class MajorUM
    {
        [Required]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public String Name { get; set; }

        [StringLength(256)]
        public String Description { get; set; }
    }
}
