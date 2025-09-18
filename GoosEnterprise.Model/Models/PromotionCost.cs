using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace GoodsEnterprise.Model.Models
{
    public partial class PromotionCost
    {
        public int PromotionCostId { get; set; }
        
        [Required(ErrorMessage = "Product is required")]
        public int? ProductId { get; set; }
        
        [Required(ErrorMessage = "Promotion Cost is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Promotion Cost must be greater than 0")]
        public decimal? PromotionCost1 { get; set; }
        
        [Required(ErrorMessage = "Start Date is required")]
        public DateTime? StartDate { get; set; }
        
        [Required(ErrorMessage = "End Date is required")]
        public DateTime? EndDate { get; set; }
        public string Remark { get; set; }
        
        [Required(ErrorMessage = "Supplier is required")]
        public int? SupplierId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? Modifiedby { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDelete { get; set; }
        public virtual Product Product { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}
