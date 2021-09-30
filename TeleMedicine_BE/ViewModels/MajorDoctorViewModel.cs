using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ViewModels
{
    public enum MajorDoctorFieldEnum
    {
        Id,
        DoctorId,
        MajorId
    }
    public class MajorDoctorVM
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int MajorId { get; set; }

        public virtual MajorVM Major { get; set; }
    }

    public class MajorDoctorWithRegisterCM
    {

        [Required]
        public int MajorId { get; set; }
    }

    public class MajorDoctorCM
    {
        [Required]
        public int DoctorId { get; set; }
       
        [Required]
        public int MajorId { get; set; }
    }

    public class MajorDoctorUM
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int MajorId { get; set; }
    }
}
