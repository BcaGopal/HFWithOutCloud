﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity ;
using Microsoft.AspNet.Identity.EntityFramework ;
namespace Model.Models
{
    public class RolesDocType : EntityBase, IHistoryLog
    {
        [Key]
        public int RolesDocTypeId { get; set; }
        [Display(Name = "Role"),MaxLength(128)]
        [ForeignKey("Role")]
        public string RoleId { get; set; }        
        public virtual IdentityRole Role { get; set; }
        
        [ForeignKey("DocType")]
        public int? DocTypeId { get; set; }
        public virtual DocumentType DocType { get; set; }

        [ForeignKey("ProductType")]
        public int? ProductTypeId { get; set; }
        public virtual ProductType ProductType { get; set; }

        [ForeignKey("Menu")]
        public int? MenuId { get; set; }
        public virtual Menu Menu { get; set; }

        [Required]
        public string ControllerName { get; set; }
        [Required]
        public string ActionName { get; set; }
        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }
        [Display(Name = "Created Date"),DisplayFormat(DataFormatString="{0:dd/MMM/yyyy}")]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Modified Date"),DisplayFormat(DataFormatString="{0:dd/MMM/yyyy}")]
        public DateTime ModifiedDate { get; set; }
    }
}
