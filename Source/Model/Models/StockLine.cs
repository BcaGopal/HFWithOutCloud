﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class StockLine : EntityBase, IHistoryLog, IProductUidLastStatus
    {
        public StockLine()
        {
        }

        [Key]        
        public int StockLineId { get; set; }

        [Display(Name = "Purchase Indent")]
        [ForeignKey("StockHeader")]
        public int StockHeaderId { get; set; }
        public virtual StockHeader  StockHeader { get; set; }

        [ForeignKey("ProductUid"), Display(Name = "ProductUid")]
        public int ? ProductUidId { get; set; }
        public virtual ProductUid ProductUid { get; set; }

        [ForeignKey("RequisitionLine"), Display(Name = "Request No")]
        public int? RequisitionLineId { get; set; }
        public virtual RequisitionLine RequisitionLine { get; set; }

        [Display(Name = "Product"), Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        [Display(Name = "From Process")]
        [ForeignKey("FromProcess")]
        public int ? FromProcessId { get; set; }
        public virtual Process FromProcess { get; set; }

        [Display(Name = "Lot No."), MaxLength(10)]
        public string LotNo { get; set; }

        [Display(Name = "Plan No."), MaxLength(50)]
        public string PlanNo { get; set; }

        [Display(Name = "Qty"), Required]
        public Decimal Qty { get; set; }

        [Display(Name="Remark")]
        public string Remark { get; set; }


        [Display(Name = "From Stock")]
        [ForeignKey("FromStock")]
        public int? FromStockId { get; set; }
        public virtual Stock FromStock { get; set; }


        [Display(Name = "Stock")]
        [ForeignKey("Stock")]
        public int? StockId { get; set; }
        public virtual Stock Stock { get; set; }


        [Display(Name = "Stock Process")]
        [ForeignKey("StockProcess")]
        public int? StockProcessId { get; set; }
        public virtual StockProcess StockProcess { get; set; }


        [Display(Name = "From Stock Process")]
        [ForeignKey("FromStockProcess")]
        public int? FromStockProcessId { get; set; }
        public virtual StockProcess FromStockProcess { get; set; }

        [ForeignKey("Dimension1"), Display(Name = "Dimension1")]
        public int? Dimension1Id { get; set; }
        public virtual Dimension1 Dimension1 { get; set; }

        [ForeignKey("Dimension2"), Display(Name = "Dimension2")]
        public int? Dimension2Id { get; set; }
        public virtual Dimension2 Dimension2 { get; set; }


        [ForeignKey("Dimension3"), Display(Name = "Dimension3")]
        public int? Dimension3Id { get; set; }
        public virtual Dimension3 Dimension3 { get; set; }

        [ForeignKey("Dimension4"), Display(Name = "Dimension4")]
        public int? Dimension4Id { get; set; }
        public virtual Dimension4 Dimension4 { get; set; }

        [MaxLength(50)]
        public string Specification { get; set; }
        public decimal Rate { get; set; }

        public decimal Amount { get; set; }

        [ForeignKey("CostCenter"), Display(Name = "Cost Center")]
        public int? CostCenterId { get; set; }
        public virtual CostCenter CostCenter { get; set; }
        [MaxLength(1)]
        public string DocNature { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }

        [Display(Name = "Lock Reason")]
        public string LockReason { get; set; }


        [MaxLength(50)]
        public string OMSId { get; set; }

        public int? Sr { get; set; }

        [Display(Name = "Weight")]
        public Decimal? Weight { get; set; }

        [Display(Name = "StockIn")]
        [ForeignKey("StockIn")]
        public int? StockInId { get; set; }
        public virtual Stock StockIn { get; set; }



        /// <summary>
        /// ///////////////For Maintaining Product Uid Last Values///////////////////////////
        /// </summary>
        public int? ProductUidLastTransactionDocId { get; set; }

        public string ProductUidLastTransactionDocNo { get; set; }

        [ForeignKey("ProductUidLastTransactionDocType"), Display(Name = "Document Type")]
        public int? ProductUidLastTransactionDocTypeId { get; set; }
        public virtual DocumentType ProductUidLastTransactionDocType { get; set; }

        public DateTime? ProductUidLastTransactionDocDate { get; set; }

        [ForeignKey("ProductUidLastTransactionPerson"), Display(Name = "Last Transaction Person")]
        public int? ProductUidLastTransactionPersonId { get; set; }
        public virtual Person ProductUidLastTransactionPerson { get; set; }

        [ForeignKey("ProductUidCurrentGodown"), Display(Name = "Current Godown")]
        public int? ProductUidCurrentGodownId { get; set; }
        public virtual Godown ProductUidCurrentGodown { get; set; }

        [ForeignKey("ProductUidCurrentProcess"), Display(Name = "Current Process")]
        public int? ProductUidCurrentProcessId { get; set; }
        public virtual Process ProductUidCurrentProcess { get; set; }

        [MaxLength(10)]
        public string ProductUidStatus { get; set; }

        [ForeignKey("ReferenceDocType"), Display(Name = "ReferenceDocType")]
        public int? ReferenceDocTypeId { get; set; }
        public virtual DocumentType ReferenceDocType { get; set; }

        public int? ReferenceDocId { get; set; }

        public int? ReferenceDocLineId { get; set; }

    }
}
