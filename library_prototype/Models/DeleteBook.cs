using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace library_prototype.Models
{
    public class DeleteBook
    {
        [Required]
        public Guid BookId { get; set; }
    }
}
