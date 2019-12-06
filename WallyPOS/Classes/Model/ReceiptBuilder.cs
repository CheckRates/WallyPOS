using System.IO;

namespace WallyPOS.Classes.Model
{
    public class ReceiptBuilder
    {
        public string filepath { get; set; }

        public ReceiptBuilder(string where)
        {
            filepath = where;
        }

        public void CreateReceipt(CustomerOrder allOrderInfo)
        {
            // Create directory if doesn't exists
            DirectoryInfo di = Directory.CreateDirectory(filepath);

            string filename = "Order_" + allOrderInfo.order.OrderId + ".txt";
            filepath += "\\" + filename;

            using (StreamWriter file = new System.IO.StreamWriter(filepath))
            {
                file.WriteLine("*********************");
                file.WriteLine("Wally’s World " + allOrderInfo.branch.BranchName);
                file.WriteLine("On " + allOrderInfo.order.OrderDate.ToString() + ", " + allOrderInfo.customer.FullName);
                file.WriteLine("Order ID: " + allOrderInfo.order.OrderId);

                foreach(var orderLine in allOrderInfo.order.ProductsInOrder)
                {
                    file.WriteLine(orderLine.ItemName + "("+  orderLine.ItemType+") " +
                                    orderLine.quantity + " x " + orderLine.UnitPrice);
                }

                file.WriteLine("Subtotal = $ " + string.Format("{0:N2}", allOrderInfo.order.SubTotal));
                file.WriteLine("HST (" + (int)(Order.taxesMult * 100) + "%) = $" + string.Format("{0:N2}", allOrderInfo.order.OnlyTaxes));
                file.WriteLine("Sale Total = $" + string.Format("{0:N2}", allOrderInfo.order.TotalBalance));
                file.WriteLine("Paid - Thank you!");
            }
        }
    }
}