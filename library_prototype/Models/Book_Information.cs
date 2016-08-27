using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace library_prototype.Models
{
    public class Book_Information
    {
        [Required]
        [StringLength(150, MinimumLength = 6)]
        public String Title { get; set; }

        [Required]
        [MinLength(3)]
        public int CallNumber { get; set; }

        [MinLength(1)]
        public int Volume { get; set; }

    }
}