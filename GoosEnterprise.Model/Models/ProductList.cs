using System;
using System.Collections.Generic;
using System.Text;

namespace GoodsEnterprise.Model.Models
{
    public class ProductList
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string OuterEan { get; set; }
        public int FilterTotalCount { get; set; }
        public string Status { get; set; }
    }
}
