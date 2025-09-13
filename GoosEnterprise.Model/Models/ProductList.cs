using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GoodsEnterprise.Model.Models
{
    public class ProductList
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string ProductName { get; set; }
        public string OuterEan { get; set; }
        public int FilterTotalCount { get; set; }
        public string Status { get; set; }
    }

    public class PromotionCostList
    {
        [Key]
        public int PromotionCostID { get; set; }
        public string SupplierName { get; set; }
        public string ProductName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int FilterTotalCount { get; set; }
        public string Status { get; set; }
    }
}
