﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DTShopping.Models
{
    public class CartFilter
    {
        public string username { get; set; }

        public string password { get; set; }

        public int userId { get; set; }

        public int companyId { get; set; }

        public int productId { get; set; }

        public int quantity { get; set; }

        public int deliveryType { get; set; }

        public string Size { get; set; }

        public string Color { get; set; }
         public decimal hwallet { get; set; }
    }
}