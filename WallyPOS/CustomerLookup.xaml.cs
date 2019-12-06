using WallyPOS.Classes.ViewModel;
using WallyPOS.Classes.Model;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;

namespace WallyPOS
{
    /// <summary>
    /// Interaction logic for CustomerLookup.xaml
    /// </summary>
    public partial class CustomerLookup : Window
    {
        public CustomerLookup()
        {
            InitializeComponent();
            DataContext = new CustomerLookupVM();

            var tempVM = (CustomerLookupVM)this.DataContext;
            tempVM.FilterCustomers("", "", ""); 
        }


        private void CreateCustomer_Click(object sender, RoutedEventArgs e)
        {
            // Clear any error message
            ErrorMessage.Visibility = Visibility.Hidden;

            // Get new customer parameters from the text box
            string firstName = InFistName.Text.ToString();
            string lastName = InLastName.Text.ToString();
            string phoneNum = InPhoneNum.Text.ToString();

            // Input field level validation
            if (firstName == "")
            {
                ErrorMessage.Visibility = Visibility.Visible;
                ErrorMessage.Content = "First name cannot be blank!";
                return;
            }
            else if (lastName == "")
            {
                ErrorMessage.Visibility = Visibility.Visible;
                ErrorMessage.Content = "Last name cannot be blank!";
                return;
            }
            else if (phoneNum == "")
            {
                ErrorMessage.Visibility = Visibility.Visible;
                ErrorMessage.Content = "Phone number cannot be blank!";
                return;
            }

            // Create New Customer
            var tempVM = (CustomerLookupVM)this.DataContext;
            tempVM.CreateCustomer(firstName, lastName, phoneNum);

            SetErrorMessage("Customer Created!", true);

            // Clear Text fields
            InFistName.Text = "";
            InLastName.Text = "";
            InPhoneNum.Text = "";
        }

        private void CustomerFilters_Click(object sender, RoutedEventArgs e)
        {
            // Get filters from textboxes 
            string firstName = FirstNameFilter.Text.ToString();
            string lastName = LastNameFilter.Text.ToString();
            string phoneNum = PhoneNumFilter.Text.ToString();

            // Access ViewModel
            var tempVM = (CustomerLookupVM)this.DataContext;
            tempVM.FilterCustomers(firstName, lastName, phoneNum);
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.foundCustomer = (Customer)FoundCustomers.SelectedItem;
            this.Close();
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
    }
}
