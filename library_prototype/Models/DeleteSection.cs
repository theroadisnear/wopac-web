using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace library_prototype.Models
{
    public class DeleteSection
    {
        [Required]
        public Guid SectionId { get; set; }
    }
}