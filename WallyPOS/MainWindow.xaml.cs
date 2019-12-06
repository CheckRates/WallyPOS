using System.Windows;
using System.Collections.Generic;
using WallyPOS.Classes.DataLayer;
using WallyPOS.Classes.Model;
using WallyPOS.Classes.ViewModel;
using System;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Media;

namespace WallyPOS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {   
        private static readonly Regex onlyNumbers = new Regex("^[0-9]*$");
        public static Customer foundCustomer { get; set; }

        public MainWindow()
        {
            // Initialize and assign MainWindow ViewModel
            InitializeComponent();
            DataContext = new MainWindowVM();

            var tempVM = (MainWindowVM)this.DataContext;
            tempVM.FilterCustomersOrder("", "", "", "");
            tempVM.FilterProducts("");
        }

        private void CustomerLookUp_Click(object sender, RoutedEventArgs e)
        {
            // Start new window
            Window customerLookup = new CustomerLookup();
            customerLookup.Owner = this;
            customerLookup.ShowDialog();

            LookUpButton.Content = "Create / Look Up";
            if (foundCustomer != null)
            {
                LookUpButton.Content = foundCustomer.FirstName + " " + foundCustomer.LastName;
            }    
        }

        private void CartAddOrder_Click(object sender, RoutedEventArgs e)
        {
            // Clear Error message
            ErrorMessage.Visibility = Visibility.Hidden;

            // Grab Parameters
            Item selectedProduct = (Item)ProductsList.SelectedItem;
            int quantity = 0;

            if (onlyNumbers.IsMatch(AddQuantity.Text))
            {
                quantity = Convert.ToInt32(AddQuantity.Text);
            }
            else 
            {
                quantity = -1;
            }

            // Input level validation
            if(selectedProduct == null)
            {
                SetErrorMessage("Please select an item to add to the cart!");
                return;
            }
            else if(quantity < 0)
            {
                SetErrorMessage("Product quantity needs to be higher than zero!");
                return;
            }

            // Send to ViewModel layer and add product to cart
            var tempVM = (MainWindowVM)this.DataContext;
            if(!tempVM.AddProductToCart(selectedProduct, quantity))
            {
                SetErrorMessage("Total product quantity cannot be higher than the stock!");
            }

            // Clean UI
            AddQuantity.Text = "";
        }

        private void CartRemove_Click(object sender, RoutedEventArgs e)
        {
            // Clear Error message
            ErrorMessage.Visibility = Visibility.Hidden;

            // Grab Parameters
            ShoppingCartItem selectedCartItem = (ShoppingCartItem)CartList.SelectedItem;


            // Input level validation
            if (selectedCartItem == null)
            {
                SetErrorMessage("Please select an item to remove off the cart!");
                return;
            }

            // Send to ViewModel layer and add remove to cart
            var tempVM = (MainWindowVM)this.DataContext;
            tempVM.RemoveProductOffCart(selectedCartItem);
        }

        private void SetErrorMessage(string message, bool success = false)
        {
            ErrorMessage.Visibility = Visibility.Visible;
            ErrorMessage.Background = new SolidColorBrush(Colors.Red);
            if (success == true)
            {
                // Set the colour to Red if success...haha get it? (explanation: im colorblind)
                ErrorMessage.Background = new SolidColorBrush(Colors.Green);
            }
            ErrorMessage.Content = message;
        }

        private void CreateOrder_Click(object sender, RoutedEventArgs e)
        {
            // Clear Error message
            ErrorMessage.Visibility = Visibility.Hidden;

            // Get Branch
            Branch selectedBranch = (Branch)BranchSelection.SelectedItem;

            // Input level validation
            if (selectedBranch == null)
            {
                SetErrorMessage("Please select the branch of the order!");
                return;
            }
            else if (foundCustomer == null)
            {
                SetErrorMessage("Please select the customer of the order!");
                return;
            }

            // Send to ViewModel layer and create order
            var tempVM = (MainWindowVM)this.DataContext;
            if (!tempVM.CreateNewOrder(foundCustomer, selectedBranch))
            {
                SetErrorMessage("Shopping cart cannot be blank!");
                return;
            }

            SetErrorMessage("Order and Receipt Created!", true);
        }

        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            // Seacrh By Order Id 
            int orderId = -1;

            if (InOrderId.Text.ToString() != "" && onlyNumbers.IsMatch(InOrderId.Text))
            {
                orderId = Convert.ToInt32(InOrderId.Text);
            }

            // Get filters from textboxes 
            string firstName = FirstNameFilter.Text.ToString();
            string lastName = LastNameFilter.Text.ToString();
            string phoneNum = PhoneNumFilter.Text.ToString();
            string branchName = BranchFilter.Text.ToString();

            // Access ViewModel
            var tempVM = (MainWindowVM)this.DataContext;
            if (orderId == -1)
            {
                tempVM.FilterCustomersOrder(firstName, lastName, phoneNum, branchName);
            }
            else
            {
                tempVM.GetOneCustomerOrder(orderId);
            }
        }

        private void CustomerOrder_DoubleClick(object sender, RoutedEventArgs e)
        {
            // Get Selected Order
            var selectedOrder = (CustomerOrder)Orders.SelectedItem;
            int orderId = selectedOrder.order.OrderId;

            // Start new window
            Window productsOrder = new ProductsOrder(orderId);
            productsOrder.Owner = this;
            productsOrder.ShowDialog();
        }

        private void SearchProduct_Click(object sender, RoutedEventArgs e)
        {
            // Get filters from textboxes 
            string productName = ProductNameFilter.Text.ToString();

            // Access ViewModel
            var tempVM = (MainWindowVM)this.DataContext;
            tempVM.FilterProducts(productName);
        }
    }
}
