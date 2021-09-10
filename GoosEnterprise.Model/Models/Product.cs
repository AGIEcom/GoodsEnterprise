using System;
using System.Collections.Generic;

#nullable disable

namespace GoodsEnterprise.Model.Models
{
    public partial class Product
    {
        public Product()
        {
            CustomerBaskets = new HashSet<CustomerBasket>();
            CustomerFavourites = new HashSet<CustomerFavourite>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
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
        public string Image { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Createdby { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? Modifiedby { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int? SupplierId { get; set; }
        public DateTime? ExpriyDate { get; set; }

        public virtual ICollection<CustomerBasket> CustomerBaskets { get; set; }
        public virtual ICollection<CustomerFavourite> CustomerFavourites { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
