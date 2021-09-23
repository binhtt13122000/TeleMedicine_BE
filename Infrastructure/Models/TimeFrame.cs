using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Models
{
    public partial class TimeFrame
    {
        public int Id { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
