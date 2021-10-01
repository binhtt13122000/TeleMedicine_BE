using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ViewModels
{
    public enum SlotFieldEnum
    {
        Id,
        AssignedDate,
        DoctorId,
        StartTime,
        EndTime,
        HealthCheckId
    }

    public class SlotHealthCheckVM
    {
        public int Id { get; set; }
        public DateTime AssignedDate { get; set; }
        public int DoctorId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class SlotVM
    {
        public int Id { get; set; }
        public DateTime AssignedDate { get; set; }
        public int DoctorId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int? HealthCheckId { get; set; }

        public virtual DoctorVM Doctor { get; set; }
        public virtual HealthCheck HealthCheck { get; set; }
    }

    public class SlotCM
    {
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime AssignedDate { get; set; }
        
        [Required]
        public int DoctorId { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

    }

    public class SlotHeakthCheckCM
    {
        [Required]
        public int Id;

    }


    public class SlotUM
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime AssignedDate { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }
    }
}
