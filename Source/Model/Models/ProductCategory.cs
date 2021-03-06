﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class ProductCategory : EntityBase, IHistoryLog
    {
        public ProductCategory()
        {
        }

        [Key]
        public int ProductCategoryId { get; set; }

        [Display(Name="Product Category Name")]
        [MaxLength(50, ErrorMessage = "ProductCategory Name cannot exceed 50 characters"), Required]
        [Index("IX_ProductCategory_ProductCategoryName", IsUnique = true)]
        public string ProductCategoryName { get; set; }
        
        [ForeignKey("ProductType")]
        [Display(Name = "Product Type")]
        public int ProductTypeId { get; set; }
        public virtual ProductType ProductType { get; set; }

        [ForeignKey("DefaultSalesTaxProductCode")]
        [Display(Name = "Default Sales Tax Product Code")]
        public int? DefaultSalesTaxProductCodeId { get; set; }
        public virtual SalesTaxProductCode DefaultSalesTaxProductCode { get; set; }

        [Display(Name = "Is System Define ?")]
        public Boolean IsSystemDefine { get; set; }

        [Display(Name = "Is Active ?")]
        public Boolean IsActive { get; set; }
        [MaxLength(100)]
        public string ImageFolderName { get; set; }

        [MaxLength(100)]
        public string ImageFileName { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }

        [MaxLength(50)]
        public string OMSId { get; set; }
    }
}
