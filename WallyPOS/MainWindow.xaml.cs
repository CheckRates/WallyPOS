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

        private void SetErrorMessage(string message)
        {
            ErrorMessage.Visibility = Visibility.Visible;
            ErrorMessage.Content = message;
        }

        private void CreateOrder_Click(object sender, RoutedEventArgs e)
        {
            // Get Branch
            Branch selectedBranch = (Branch)BranchSelection.SelectedItem;

            // Send to ViewModel layer and create order
            var tempVM = (MainWindowVM)this.DataContext;
            tempVM.CreateNewOrder(foundCustomer, selectedBranch);
        }

        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CustomerOrder_DoubleClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
