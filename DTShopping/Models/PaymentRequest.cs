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

      public class paymentRequestGoHappy
    {
        public string Formno { get; set; }
         public decimal Amount { get; set; }
         public string OrderNo { get; set; }
         public string Status { get; set; }
        public string TDate { get; set; }
        public string TID { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ID { get; set; }
        public string Response { get; set; }
    }

    public class APIPaymentResponseModel
    {
        public string FormNo { get; set; }
        public string TID { get; set; }
        public string Amount { get; set; }
        public string Status { get; set; }
    }
}