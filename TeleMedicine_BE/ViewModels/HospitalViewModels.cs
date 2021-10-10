using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ViewModels
{
    public enum HospitalFieldEnum
    {
        Id,
        HospitalCode,
        Name,
        Address,
    }
    public class HospitalVM
    {
        public int Id { get; set; }
        public string HospitalCode { get; set; }
        public string Name { get; set; }

        public string Address { get; set; }
        public string Description { get; set; }
        public double Lat { get; set; }

        public double Long { get; set; }

        public bool? IsActive { get; set; }
    }

    public class HospitalCM
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string HospitalCode { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Address { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        [Required]
        public double Lat { get; set; }

        [Required]
        public double Long { get; set; }
    }

    public class HospitalUM
    {
        [Required]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string HospitalCode { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Address { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        [Required]
        public double Lat { get; set; }

        [Required]
        public double Long { get; set; }

        [Required]
        public bool? IsActive { get; set; }
    }

    public class HospitalWithDoctorVM
    {
        public int Id { get; set; }
        public string HospitalCode { get; set; }
        public string Name { get; set; }

        public string Address { get; set; }
        public string Description { get; set; }

        public bool? IsActive { get; set; }

        public virtual ICollection<HospitalDoctor> HospitalDoctors { get; set; }
    }
}
