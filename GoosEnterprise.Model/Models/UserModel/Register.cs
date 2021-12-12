using System;
using System.Collections.Generic;
using System.Text;

namespace GoodsEnterprise.Model.Models.UserModel
{
    public class Register
    {
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
        public int RoleId { get; set; }
        public bool EmailSubscribed { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Createdby { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? Modifiedby { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }
    }
}
