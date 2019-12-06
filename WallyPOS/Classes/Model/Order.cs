using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WallyPOS.Classes.Model
{
    public class Order : INotifyPropertyChanged
    {
        // Constants 
        public const double taxesMult = 0.13;
        // Events
        public event PropertyChangedEventHandler PropertyChanged;

        //----------ATTRIBUTES----------//
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int BranchId { get; set; }
        public DateTime OrderDate { get; set; }
        public bool PaymentAuthStatus { get; set; }
        private List<ShoppingCartItem> _productsInOrder = null;
        public List<ShoppingCartItem> ProductsInOrder
        {
            get { return _productsInOrder; }
            set
            {
                _productsInOrder = value;
                foreach(ShoppingCartItem products in _productsInOrder)
                {
                    SubTotal += products.sPrice;
                }
                OnlyTaxes = SubTotal * taxesMult;
                TotalBalance = SubTotal + OnlyTaxes;
                OnPropertyChanged("ProductsInOrder");
            }
        }

        private double _subTotal = 0;
        public double SubTotal
        {
            get { return _subTotal; }
            set
            {
                _subTotal = value;
            }
        }

        private double _onlyTaxes = 0;
        public double OnlyTaxes
        {
            get { return _onlyTaxes; }
            set
            {
                _onlyTaxes = value;
            }
        }

        private double _totalBalance = 0;
        public double TotalBalance
        {
            get { return _totalBalance; }
            set
            {
                _totalBalance = value;
            }
        }






        //----------METHODS----------//
        public Order(int customerId, int branchId)
        {
            CustomerId = customerId;
            BranchId = branchId;
            OrderDate = DateTime.Now;
        }

        public Order()
        { }

        // Handlers
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}