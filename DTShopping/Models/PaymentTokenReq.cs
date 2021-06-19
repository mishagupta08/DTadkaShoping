using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTShopping.Models
{
    public class PaymentTokenReq
    {
        public string amount { get; set; }
        public string contact_number { get; set; }
        public string email_id { get; set; }
        public string currency { get; set; }
        public string companyId { get; set; }
        public bool isSandBox { get; set; }
    }
}