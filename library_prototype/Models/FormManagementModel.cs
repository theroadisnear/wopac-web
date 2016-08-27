using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace library_prototype.Models
{
    public class FormManagementModel
    {
        public class FormModel
        {
            public Guid Id { get; set; }
            [StringLength(450)]
            [Index(IsUnique = true)]
            public string Grade { get; set; }
            [StringLength(450)]
            [Index(IsUnique = true)]
            public string Section { get; set; }
        }
    }
}