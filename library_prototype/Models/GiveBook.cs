using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace library_prototype.Models
{
    public class GiveBook
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid BookId { get; set; }
        [Required]
        public DateTime Deadline { get; set; }
    }
}
