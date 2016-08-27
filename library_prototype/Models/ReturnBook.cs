using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace library_prototype.Models
{
    public class ReturnBook
    {
        [Required]
        public Guid BookLogId { get; set; }
        [Required]
        public bool Return { get; set; }
        [Required]
        public bool BookStatus { get; set; }
        public string MessageLog { get; set; }
    }
}
