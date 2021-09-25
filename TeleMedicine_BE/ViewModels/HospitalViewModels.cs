using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ViewModels
{
    public class HospitalVM
    {
        public int Id { get; set; }
        public String HospitalCode { get; set; }
        public String Name { get; set; }

        public String Address { get; set; }
        public String Description { get; set; }
    }

    public class HospitalCM
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public String HospitalCode { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public String Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        public String Address { get; set; }

        [StringLength(256)]
        public String Description { get; set; }
    }

    public class HospitalUM
    {
        [Required]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public String HospitalCode { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public String Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        public String Address { get; set; }

        [StringLength(256)]
        public String Description { get; set; }
    }

    public class HospitalWithDoctorVM
    {
        public int Id { get; set; }
        public String HospitalCode { get; set; }
        public String Name { get; set; }

        public String Address { get; set; }
        public String Description { get; set; }

        public virtual ICollection<HospitalDoctor> HospitalDoctors { get; set; }
    }
}
