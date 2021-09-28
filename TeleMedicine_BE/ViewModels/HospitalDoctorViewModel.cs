using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ViewModels
{
    public class HospitalDoctorVM
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int HospitalId { get; set; }
        public bool IsWorking { get; set; }

        public virtual HospitalVM Hospital { get; set; }
    }

    public class HospitalDoctorWithRegisterCM
    {

        [Required]
        public int HospitalId { get; set; }

    }

    public class HospitalDoctorCM
    {
        [Required]
        public int DoctorId { get; set; }
       
        [Required]
        public int HospitalId { get; set; }
        
    }

    public class HospitalDoctorUM
    {
        [Required]
        public int Id;

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int HospitalId { get; set; }

        [Required]
        public bool IsWorking { get; set; }

    }
}
