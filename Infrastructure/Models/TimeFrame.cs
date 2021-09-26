using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Models
{
    public partial class TimeFrame
    {
        public int Id { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}
