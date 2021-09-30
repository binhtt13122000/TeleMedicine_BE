using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeleMedicine_BE.ViewModels
{
    public class HealthCheckVM
    {
        public int Id { get; set; }
        public int? Height { get; set; }
        public int? Weight { get; set; }
        public string ReasonCancel { get; set; }
        public int? Rating { get; set; }
        public string Comment { get; set; }
        public string Advice { get; set; }
        public string Token { get; set; }
        public int PatientId { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? CanceledTime { get; set; }
    }
}
