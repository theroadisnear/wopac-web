using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace library_prototype.Models
{
    public class UpdateSection
    {
        [Required]
        public Guid SectionId { get; set; }
        [StringLength(50, MinimumLength = 5)]
        [Display(Name = "Section Name*")]
        public string SectionName { get; set; }
    }
}