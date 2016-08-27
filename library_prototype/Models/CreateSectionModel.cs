using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace library_prototype.Models
{
    public class CreateSectionModel
    {
        [Required]
        [Display(Name = "Section Name*")]
        [StringLength(50, MinimumLength = 2)]
        public string Section { get; set; }
        
    }
}