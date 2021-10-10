using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ViewModels
{
    public enum RoleFieldEnum
    {
        Id,
        Name
    }
    public class RoleVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class RoleUM
    {
        [Required]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string Name { get; set; }

        [Required]
        public bool? IsActive { get; set; }
    }

    public class RoleCM
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string Name { get; set; }
    }
}
