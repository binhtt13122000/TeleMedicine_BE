using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ViewModels
{

    public enum DoctorFieldEnum
    {
        Id,
        Email,
        PractisingCertificate,
        CertificateCode,
        PlaceOfCertificate,
        DateOfCertificate,
        ScopeOfPractice,
        NumberOfConsultants,
        Rating,
    }
    public class DoctorVM
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Avatar { get; set; }

        public string PractisingCertificate { get; set; }

        public string CertificateCode { get; set; }

        public string PlaceOfCertificate { get; set; }

        public DateTime DateOfCertificate { get; set; }

        public string ScopeOfPractice { get; set; }

        public string Description { get; set; }

        public int? NumberOfConsultants { get; set; }

        public int? NumberOfCancels { get; set; }
        public double? Rating { get; set; }
        public bool? IsVerify { get; set; }

        public bool? IsActive { get; set; }

        public virtual ICollection<CertificationDoctorVM> CertificationDoctors { get; set; }
        public virtual ICollection<HospitalDoctorVM> HospitalDoctors { get; set; }
        public virtual ICollection<MajorDoctorVM> MajorDoctors { get; set; }

    }

    public class DoctorSM
    {

        public string Email { get; set; }

        public string Name { get; set; }

        public string Avatar { get; set; }

        public string PractisingCertificate { get; set; }

        public string CertificateCode { get; set; }

        public string PlaceOfCertificate { get; set; }

        public DateTime DateOfCertificate { get; set; }

        public string ScopeOfPractice { get; set; }

        public string Description { get; set; }
    }

    public class DoctorCertificationCM
    {
        [Required]
        public CertificationDoctorWithRegisterCM certification { get; set; }
    }

    public class DoctorCM
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(128)]
        public string Email { get; set; }

        [Required]
        public IFormFile PractisingCertificate { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string CertificateCode { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string PlaceOfCertificate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfCertificate { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ScopeOfPractice { get; set; }

        public string Description { get; set; }

        public string  HospitalDoctors { get; set; }
        public string MajorDoctors { get; set; }
    }



    public class DoctorUM
    {
        [Required]
        public int Id { get; set; }

        public IFormFile PractisingCertificate { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string CertificateCode { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string PlaceOfCertificate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfCertificate { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string ScopeOfPractice { get; set; }

        public string Description { get; set; }

        [Required]
        public bool? IsActive { get; set; }

        public string HospitalDoctors { get; set; }
        public string MajorDoctors { get; set; }
    }
}
