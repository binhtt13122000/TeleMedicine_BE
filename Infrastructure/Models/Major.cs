using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Models
{
    public partial class Major
    {
        public Major()
        {
            MajorDoctors = new HashSet<MajorDoctor>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public bool? IsActive { get; set; }

        public virtual ICollection<MajorDoctor> MajorDoctors { get; set; }
    }
}
