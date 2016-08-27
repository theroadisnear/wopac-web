using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace library_prototype.Models
{
    public class ImageModel
    {
        [Display(Name = "Image")]
        [Required]
        public string Path { get; set; }
        [Required]
        public string Name { get; set; }
    }
}