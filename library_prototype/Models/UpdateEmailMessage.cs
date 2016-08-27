using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace library_prototype.Models
{
    public class UpdateEmailMessage
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        [EmailAddress]
        public string Sender { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
