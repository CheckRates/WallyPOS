﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using WallyPOS.Classes.DataLayer;
using WallyPOS.Classes.Model;

namespace WallyPOS.Classes.ViewModel
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        // Events
        public event PropertyChangedEventHandler PropertyChanged;

        //----------ATTRIBUTES----------//
        public ObservableCollection<Branch> PossibleBranches { get; set; }
        public ObservableCollection<Item> Products { get; set; }
        private ObservableCollection<ShoppingCartItem> _shoppingCart;
        public ObservableCollection<ShoppingCartItem> ShoppingCart
        {
            get { return _shoppingCart; }
            set
            {
                _shoppingCart = value;
                OnPropertyChanged("ShoppingCart");
            }
        }

        private ObservableCollection<Item> _filteredProducts;
        public ObservableCollection<Item> FilteredProducts
        {
            get { return _filteredProducts; }
            set
            {
                _filteredProducts = value;
                OnPropertyChanged("FilteredProducts");
            }
        }


        private ObservableCollection<CustomerOrder> _filteredOrders;
        public ObservableCollection<CustomerOrder> FilteredOrders
        {
            get { return _filteredOrders; }
            set
            {
                _filteredOrders = value;
                OnPropertyChanged("FilteredOrders");
            }
        }


        //----------METHODS----------//
        public MainWindowVM()
        { 
            WallyDAL dal = new WallyDAL();

            PossibleBranches = new ObservableCollection<Branch>(dal.GetBranches());
            Products = new ObservableCollection<Item>(dal.GetProductsByName(""));
            _shoppingCart = new ObservableCollection<ShoppingCartItem>();
            FilteredOrders = new ObservableCollection<CustomerOrder>();
        }

        public bool AddProductToCart(Item selectedProduct, int quantity)
        {
            bool wasProductAdded = false;
            int existingItemIndex = -1;

            // Item already exists in cart
            if ((existingItemIndex = AlreadyInCart(selectedProduct)) != -1)            
            {
                wasProductAdded = ShoppingCart[existingItemIndex].AddToQuantity(quantity);
                OnPropertyChanged("ShoppingCart");
            }
            // New item
            else if(quantity <= selectedProduct.ItemStock)
            {
                var addedProduct = new ShoppingCartItem(selectedProduct, quantity);
                ShoppingCart.Add(addedProduct);                                    
                wasProductAdded = true;
               
                OnPropertyChanged("ShoppingCart");
            }

            return wasProductAdded;
        }

        public void RemoveProductOffCart(ShoppingCartItem selectedCartItem)
        {
            ShoppingCart.Remove(selectedCartItem);
            OnPropertyChanged("ShoppingCart");
        }
        
        /// <summary>
        /// Sums all the quantity of the same item id if it doesn't suppress the item stock 
        /// </summary>
        /// <param name="selectedProduct" <b>Item</b> - Item that will be grouped together></param>
        /// <returns></returns>
        private int AlreadyInCart(Item selectedProduct)
        {
            int alreadyExistsIndex = -1;

            for (int counter = 0; counter < ShoppingCart.Count; counter++)
            {
                if (ShoppingCart[counter].ItemId == selectedProduct.ItemId)
                {
                    alreadyExistsIndex = counter;
                    break;
                }
            }

            return alreadyExistsIndex;
        }

        public bool CreateNewOrder(Customer customer, Branch branch)
        {
            if(ShoppingCart.Count == 0)
            {
                return false;
            }

            WallyDAL dal = new WallyDAL();

            Order newOrder = new Order(customer.CustomerId, branch.BranchId);
            int orderID = dal.CreateNewOrder(newOrder);

            // Associate Order with Product with OrderLInes
            newOrder.ProductsInOrder = new List<ShoppingCartItem>(ShoppingCart);
            dal.InsertOrderLine(orderID, newOrder.ProductsInOrder);

            // Create Receipt
            ReceiptBuilder receipt = new ReceiptBuilder(@"..\..\..\Receipts");
            CustomerOrder justCreated = dal.GetCustomerOrder(orderID);
            justCreated.order.ProductsInOrder = newOrder.ProductsInOrder;
            receipt.CreateReceipt(justCreated);

            // Clear Shopping Cart
            ShoppingCart.Clear();
            return true;
        }

        public void FilterProducts(string productName)
        {
            WallyDAL dal = new WallyDAL();
            FilteredProducts = new ObservableCollection<Item>(dal.GetProductsByName(productName));
        }


        //------------------------------------------ ORDER LOOKUP STUFF--------------------------------------------//
        public void FilterCustomersOrder(string firstName, string lastName, string phoneNum, string branchName)
        {
            WallyDAL dal = new WallyDAL();
            FilteredOrders = new ObservableCollection<CustomerOrder>(dal.GetOrders(firstName, lastName, phoneNum, branchName));
        }

        public void GetOneCustomerOrder(int orderId)
        {
            WallyDAL dal = new WallyDAL();
            FilteredOrders = new ObservableCollection<CustomerOrder>();
            FilteredOrders.Add( dal.GetCustomerOrder(orderId));
        }

        public void GetProductsFromOrder(int orderId)
        {
            WallyDAL dal = new WallyDAL();

        }

        // Handlers
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
