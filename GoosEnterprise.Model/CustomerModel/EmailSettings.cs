using System;
using System.Collections.Generic;
using System.Text;

namespace GoodsEnterprise.Model.Models.CustomerModel
{
    public class EmailSettings
    {
        public string smtpAddress { get; set; }
        public int portNumber { get; set; }
        public bool enableSSL { get; set; }
        public string emailFromAddress { get; set; }
        public string password { get; set; }
    }
}
