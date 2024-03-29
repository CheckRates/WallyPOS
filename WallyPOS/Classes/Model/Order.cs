﻿using System;

namespace WallyPOS.Classes.Model
{
    public class Order
    {
        //----------ATTRIBUTES----------//
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int BranchId { get; set; }
        public DateTime OrderDate { get; set; }
        public bool PaymentAuthStatus { get; set; }

        //----------METHODS----------//
        public Order(int customerId, int branchId)
        {
            CustomerId = customerId;
            BranchId = branchId;
            OrderDate = DateTime.Now;
        }
    }
}