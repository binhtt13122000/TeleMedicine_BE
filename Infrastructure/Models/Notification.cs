﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Models
{
    public partial class Notification
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public DateTime? CreatedDate { get; set; }

        public bool? IsSeen { get; set; }

        public bool? IsActive { get; set; }

        public int? Type { get; set; }
        public virtual Account User { get; set; }
    }
}
