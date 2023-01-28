using System;
using System.Collections.Generic;

#nullable disable

namespace GoodsEnterprise.Model.Models
{
    public partial class PromotionCost
    {
        public int PromotionCostId { get; set; }
        public int? ProductId { get; set; }
        public decimal? PromotionCost1 { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Remark { get; set; }
        public int? SupplierId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? Modifiedby { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual Product Product { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}
