using System;
using System.Collections.Generic;

#nullable disable

namespace GoodsEnterprise.Model.Models
{
    public partial class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal? TotalPrice { get; set; }
        public bool? IsPricingRequried { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Createdby { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? Modifiedby { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
