using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace library_prototype.Models
{
    public class AddAuthor
    {
        [Required]
        [Display(Name = "Last Name*")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "First Name*")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(1, MinimumLength = 1)]
        [Display(Name = "Middle Initial*")]
        public string MiddleInitial { get; set; }
    }
}