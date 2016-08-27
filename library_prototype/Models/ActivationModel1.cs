using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace library_prototype.Models
{
    public class ActivationModel1
    {
        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6)]
        [Display(Name = "Password*")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6)]
        [Display(Name = "Confirm Password*")]
        [Compare("Password", ErrorMessage ="Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 6)]
        [Display(Name = "Secret Question*")]
        public string SecretQuestion { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Secret Answer*")]
        public string SecretAnswer { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{MM/DD/YYYY}")]
        [Range(typeof(DateTime), "1/1/1950", "12/31/2012")]
        [Display(Name = "Birthday*")]
        public DateTime Birthday { get; set; }


        [Required]
        [Display(Name = "Zip Code*")]
        public int ZipCode { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [Display(Name = "Address 1*")]
        public string Address1 { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [Display(Name = "Address 2*")]
        public string Address2 { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 6)]
        [Display(Name = "City*")]
        public string City { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 6)]
        [Display(Name = "Country*")]
        public string Country { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [StringLength(12, MinimumLength = 7)]
        [Display(Name = "Contact Number*")]
        public string ContactNumber { get; set; }
    }
}