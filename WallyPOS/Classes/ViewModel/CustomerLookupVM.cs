using System.Collections.ObjectModel;
using System.ComponentModel;
using WallyPOS.Classes.DataLayer;
using WallyPOS.Classes.Model;


namespace WallyPOS.Classes.ViewModel
{
    public class CustomerLookupVM : INotifyPropertyChanged
    {
        // Events
        public event PropertyChangedEventHandler PropertyChanged;

        //----------ATTRIBUTES----------//
        private ObservableCollection<Customer> _filteredCustomers;
        public ObservableCollection<Customer> FilteredCustomers 
        {
            get { return _filteredCustomers; } 
            set
            {
                _filteredCustomers = value;
                OnPropertyChanged("FilteredCustomers");
            }
        }

        //----------METHODS----------//
        public CustomerLookupVM()
        { }
        
        public void FilterCustomers(string firstName, string lastName, string phoneNum)
        {
            WallyDAL dal = new WallyDAL();
            FilteredCustomers = new ObservableCollection<Customer>(dal.GetCustomers(firstName, lastName, phoneNum));
        }

        public void CreateCustomer(string firstName, string lastName, string phoneNum)
        { 
            Customer newCustomer = new Customer(firstName, lastName, phoneNum);

            WallyDAL dal = new WallyDAL();
            dal.CreateCustomer(newCustomer);
        }

        // Handlers
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
