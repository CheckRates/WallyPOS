﻿using System.ComponentModel;

namespace WallyPOS.Classes.Model
{
    public class ShoppingCartItem : Item, INotifyPropertyChanged
    {
        // Events
        public event PropertyChangedEventHandler PropertyChanged;

        //----------ATTRIBUTES----------//
        private int _quantity;
        public int quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged("quantity");
            }
        }

        private double _sPrice = 0; 
        public double sPrice
        {
            get { return _sPrice; }
            set
            {
                _sPrice = value;
            }
        }

        //----------METHODS----------//
        public ShoppingCartItem(Item selectedItem, int quantity) : base(selectedItem)
        {
            this.quantity = quantity;
            sPrice = (UnitPrice * quantity);
        }

        public ShoppingCartItem()
        { }

        public bool AddToQuantity(int more)
        {
            bool wasItAdded = false;

            if(quantity + more <= ItemStock)
            {
                quantity += more;
                wasItAdded = true;
            }

            return wasItAdded;
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
