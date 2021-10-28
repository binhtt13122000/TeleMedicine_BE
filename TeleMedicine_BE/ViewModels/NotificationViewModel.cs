using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ViewModels
{
    public enum NotificationFieldEnum
    {
        Id,
        UserId,
        CreatedDate
    }
    public class NotificationVM
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }

        public bool IsSeen { get; set; }

        public bool? IsActive { get; set; }

        public int? Type { get; set; }

        public virtual Account User { get; set; }
    }

    public class NotificationRequest
    {
        public string Token { get; set; }
        public string Email { get; set; }
    }

    public class NotificationCM
    {
        [Required(AllowEmptyStrings = false)]
        public string Content { get; set; }
        
        [Required]
        public int UserId { get; set; }

        [Required]
        public int? Type { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate { get; set; }

    }

    public class NotificationUM
    {
        [Required]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Content { get; set; }

        [Required]
        public int? Type { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate { get; set; }

        [Required]
        public bool? IsActive { get; set; }

    }
}
