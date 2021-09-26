using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ViewModels
{
    public class RoleVM
    {
        public int Id { get; set; }
        public String Name { get; set; }
    }

    public class RoleUM
    {
        [Required]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public String Name { get; set; }
    }

    public class RoleCM
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public String Name { get; set; }
    }
}
