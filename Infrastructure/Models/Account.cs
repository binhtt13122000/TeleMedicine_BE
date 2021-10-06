using System;
using System.Collections.Generic;

#nullable disable

namespace Infrastructure.Models
{
    public partial class Account
    {
        public Account()
        {
            Notifications = new HashSet<Notification>();
        }

        public int Id { get; set; }
        public string Email { get; set; }

        public string FacebookId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Ward { get; set; }
        public string StreetAddress { get; set; }
        public string Locality { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public DateTime Dob { get; set; }
        public string Avatar { get; set; }
        public bool? IsMale { get; set; }
        public int RoleId { get; set; }
        public bool? Active { get; set; }
        public DateTime? RegisterTime { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
