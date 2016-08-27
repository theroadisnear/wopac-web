using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace library_prototype.Models
{
    public class RecommendBook
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public Guid BookId { get; set; }
    }
}
