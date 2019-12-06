using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallyPOS.Classes.Model
{
    public class CustomerOrder
    {
        public Customer customer { get; set; }
        public Branch branch { get; set; }
        public Order order { get; set; }

        public CustomerOrder()
        {
            customer = new Customer();
            branch = new Branch();
            order = new Order();
        }
    }
}
