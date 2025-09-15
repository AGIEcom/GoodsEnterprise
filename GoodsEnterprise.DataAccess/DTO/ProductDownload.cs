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
        public string InnerEan { get; set; }
        public string OuterEan { get; set; }
        public string UnitSize { get; set; }
        public int? Upc { get; set; }
        public int? LayerQuantity { get; set; }
        public int? PalletQuantity { get; set; }
        public decimal? CasePrice { get; set; }
        public int? ShelfLifeInWeeks { get; set; }
        public decimal? PackHeight { get; set; }
        public decimal? PackDepth { get; set; }
        public decimal? PackWidth { get; set; }
        public decimal? NetCaseWeightKg { get; set; }
        public decimal? GrossCaseWeightKg { get; set; }
        public decimal? CaseWidthMm { get; set; }
        public decimal? CaseHeightMm { get; set; }
        public decimal? CaseDepthMm { get; set; }
        public decimal? PalletWeightKg { get; set; }
        public decimal? PalletWidthMeter { get; set; }
        public decimal? PalletHeightMeter { get; set; }
        public decimal? PalletDepthMeter { get; set; }
        public string Image { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Createdby { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? Modifiedby { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public bool isTaxable { get; set; }
        public string Supplier { get; set; }
        public DateTime? ExpriyDate { get; set; }
        public decimal? PromotionCost { get; set; }
        public DateTime? PromotionCostStartDate { get; set; }
        public DateTime? PromotionCostEndDate { get; set; }
        public string Seebelow { get; set; }
        public string Seebelow1 { get; set; }

    }
}
