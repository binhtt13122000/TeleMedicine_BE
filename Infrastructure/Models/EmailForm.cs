using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models
{
    public class EmailForm
    {
        [Required, EmailAddress]
        public string FromName { get; set; }
        [Required, EmailAddress]
        public string FromEmail { get; set; }

        [Required, EmailAddress]
        public string ToEmail { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
