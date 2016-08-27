using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace library_prototype.Models
{
    public class CreateGradeModel
    {
        [Required]
        [Display(Name = "Group Name*")]
        [StringLength(50, MinimumLength = 2)]
        public string Grade { get; set; }
        
    }
}