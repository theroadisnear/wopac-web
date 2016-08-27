using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace library_prototype.Models
{
    public class CreateBook
    {

        [Required]
        [Display(Name = "Title*")]
        public string BookTitle { get; set; }

        [Required]
        public int Volume { get; set; }

        [Required]
        [Display(Name = "Call No.*")]
        public int CallNumber { get; set; }

        [Required]
        [Display(Name = "Subject*")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "ISBN*")]
        public string ISBN { get; set; }

        [Required]
        [Display(Name = "CopyRight*")]
        public string CopyRight { get; set; }

        [Required]
        public string Synopsis { get; set; }

        [Required]
        [Display(Name = "Pages*")]
        public int NoOfPages { get; set; }

        [Required]
        [Display(Name = "Price*")]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Quantity*")]
        public int Quantity { get; set; }
        
        [Required]
        [Display(Name = "Borrow*")]
        public bool Borrow { get; set; }

        [Required]
        [Display(Name = "Publisher*")]
        public string PublisherName { get; set; }
    }
}