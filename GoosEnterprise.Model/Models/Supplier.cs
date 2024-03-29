﻿using System;
using System.Collections.Generic;

#nullable disable

namespace GoodsEnterprise.Model.Models
{
    public partial class Supplier
    {
        public Supplier()
        {
            BaseCosts = new HashSet<BaseCost>();
            PromotionCosts = new HashSet<PromotionCost>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Skucode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Createdby { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? Modifiedby { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }

        public virtual ICollection<BaseCost> BaseCosts { get; set; }
        public virtual ICollection<PromotionCost> PromotionCosts { get; set; }
    }
}
