using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace library_prototype.Models
{
    public class AuthModel
    {

        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(50, MinimumLength = 6)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength=6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name ="Remember me")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

    }
}