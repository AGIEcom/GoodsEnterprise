using System;
using System.Collections.Generic;

#nullable disable

namespace GoodsEnterprise.Model.CustomerModel
{
    public partial class HomePageCategory
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? ProductCount { get; set; }
    }
}
