using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace library_prototype.Models
{
    public class UpdateGroup
    {
        [Required]
        public Guid GroupId { get; set; }
        [StringLength(50, MinimumLength =5)]
        [Display(Name = "Group Name*")]
        public string GroupName { get; set; }
    }
}