using System.Windows;
using System.Collections.Generic;
using WallyPOS.Classes.DataLayer;
using WallyPOS.Classes.Model;
using WallyPOS.Classes.ViewModel;
using System;
using System.Text.RegularExpressions;

namespace WallyPOS
{
    /// <summary>
    /// Interaction logic for ProductsOrder.xaml
    /// </summary>
    public partial class ProductsOrder : Window 
    {
        private static readonly Regex onlyNumbers = new Regex("^[0-9]*$");
        private int DisplayedOrder = -1;

        public ProductsOrder(int orderId)
        {
            InitializeComponent();
            DisplayedOrder = orderId;
            OrderLabel.Content = DisplayedOrder;
            DataContext = new ProductsOrderVM(DisplayedOrder);
        }

        private void Refund_Click(object sender, RoutedEventArgs e)
        {
            // Clear Error message
            ErrorMessage.Visibility = Visibility.Hidden;

            ShoppingCartItem selectedProduct = (ShoppingCartItem)OrderProducts.SelectedItem;
            int quantity = 0;

            if (onlyNumbers.IsMatch(RefundQuantity.Text))
            {
                quantity = Convert.ToInt32(RefundQuantity.Text);
            }
            else
            {
                quantity = -1;
            }

            // Input level validation
            if (selectedProduct == null)
            {
                SetErrorMessage("Please select an item to refund!");
                return;
            }
            else if (quantity < 0)
            {
                SetErrorMessage("Refund quantity needs to be higher than zero!");
                return;
            }

            // Send to ViewModel layer and add product to cart
            var tempVM = (ProductsOrderVM)this.DataContext;
            tempVM.RefundItem(selectedProduct, quantity);
            OrderProducts.Items.Refresh();

            // Clean UI
            RefundQuantity.Text = "";
        }

        private void SetErrorMessage(string message)
        {
            ErrorMessage.Visibility = Visibility.Visible;
            ErrorMessage.Content = message;
        }
    }
}
