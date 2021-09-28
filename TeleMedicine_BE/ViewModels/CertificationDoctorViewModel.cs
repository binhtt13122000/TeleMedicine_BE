using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ViewModels
{
    public class CertificationDoctorVM
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int CertificationId { get; set; }
        public string Evidence { get; set; }
        public DateTime DateOfIssue { get; set; }

        public virtual CertificationVM Certification { get; set; }
    }

    public class CertificationDoctorWithRegisterCM
    {

        [Required]
        public int CertificationId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Evidence { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfIssue { get; set; }

    }

    public class CertificationDoctorCM
    {
        [Required]
        public int DoctorId { get; set; }
        
        [Required]
        public int CertificationId { get; set; }
        
        [Required(AllowEmptyStrings = false)]
        public string Evidence { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfIssue { get; set; }

    }

    public class CertificationDoctorUM
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int CertificationId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Evidence { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfIssue { get; set; }

    }

}
