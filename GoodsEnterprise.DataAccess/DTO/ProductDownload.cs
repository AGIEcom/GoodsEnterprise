using System;
using System.Collections.Generic;

#nullable disable

namespace GoodsEnterprise.Model.Models
{
    public partial class ProductDownload
    {
        public string Code { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Supplier { get; set; }
        public string InnerEan { get; set; }
        public string OuterEan { get; set; }
        public string PackSize { get; set; }
        public int? Upc { get; set; }
        public int? LayerQuantity { get; set; }
        public int? PalletQuantity { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Width { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? Depth { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Createdby { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? Modifiedby { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
       
        public DateTime? ExpriyDate { get; set; }
    }
}
