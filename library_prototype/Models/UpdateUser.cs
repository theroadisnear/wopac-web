using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace library_prototype.Models
{
    public class UpdateUser
    {
        [Required]
        [Display(Name = "First Name*")]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Middle Initial*")]
        [StringLength(1, MinimumLength = 1)]
        public string MiddleInitial { get; set; }

        [Required]
        [Display(Name = "Last Name*")]
        [StringLength(12, MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Gender*")]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "Email Address*")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }


    }
}