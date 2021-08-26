using System;
using System.Collections.Generic;

#nullable disable

namespace GoodsEnterprise.Model.Models
{
    public partial class SubCategory
    {
        public SubCategory()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Createdby { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? Modifiedby { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
