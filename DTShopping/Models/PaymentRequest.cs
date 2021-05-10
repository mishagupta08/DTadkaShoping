using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTShopping.Models
{
    public class PaymentRequest
    {
        public string UserId { get; set; }
        public decimal Amount { get; set; }
         public string OrderID { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
        public string TID { get; set; }
         public string TDate { get; set; }
         public string ID { get; set; }

    }
}