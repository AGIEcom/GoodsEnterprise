using System;
using System.Collections.Generic;

#nullable disable

namespace GoodsEnterprise.Model.Models
{
    public partial class Customer
    {
        public Customer()
        {
            CustomerBaskets = new HashSet<CustomerBasket>();
            CustomerFavourites = new HashSet<CustomerFavourite>();
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string MobilePhone { get; set; }
        public string HomePhone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Description { get; set; }
        public int? RoleId { get; set; }
        public bool? EmailSubscribed { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Createdby { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? Modifiedby { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<CustomerBasket> CustomerBaskets { get; set; }
        public virtual ICollection<CustomerFavourite> CustomerFavourites { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
