﻿using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.Reports.Models
{
    public class ReportHeader : EntityBase, IHistoryLog
    {
        public ReportHeader()
        {
            ReportLines = new List<ReportLine>();
        }
        [Key]
        public int ReportHeaderId { get; set; }
        public string ReportName { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string SqlProc { get; set; }
        public string Notes { get; set; }

        public int? ParentReportHeaderId { get; set; }        
        public string ReportSQL { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }
        public ICollection<ReportLine> ReportLines { get; set; }

    }
}