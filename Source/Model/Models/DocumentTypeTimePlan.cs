﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class DocumentTypeTimePlan : EntityBase, IHistoryLog
    {
        public DocumentTypeTimePlan()
        {
        }

        [Key]
        public int DocumentTypeTimePlanId { get; set; }

        [ForeignKey("DocType"), Display(Name = "Doc Type")]
        public int? DocTypeId { get; set; }
        public virtual DocumentType DocType { get; set; }

        [ForeignKey("Division"), Display(Name = "Division")]
        public int DivisionId { get; set; }
        public virtual Division Division { get; set; }

        [ForeignKey("Site"), Display(Name = "Site")]
        public int SiteId { get; set; }
        public virtual Site Site { get; set; }
        public string Type { get; set; }
        public decimal Days { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }
    }
}
