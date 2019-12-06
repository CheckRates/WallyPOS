using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using WallyPOS.Classes.DataLayer;
using WallyPOS.Classes.Model;

namespace WallyPOS.Classes.ViewModel
{
    public class ProductsOrderVM : INotifyPropertyChanged
    {
        // Events
        public event PropertyChangedEventHandler PropertyChanged;

        //----------ATTRIBUTES----------//
        private int DisplayedOrder = -1;
        public ObservableCollection<ShoppingCartItem> _productsInOrder { get; set; }
        public ObservableCollection<ShoppingCartItem> ProductsInOrder
        {
            get { return _productsInOrder; }
            set
            {
                _productsInOrder = value;
                OnPropertyChanged("ProductsInOrder");
            }
        }

        //----------METHODS----------//
        public ProductsOrderVM(int productsFrom)
        {
            DisplayedOrder = productsFrom;
            WallyDAL dal = new WallyDAL();
            ProductsInOrder = new ObservableCollection<ShoppingCartItem>(dal.GetProductsFromOrder(DisplayedOrder));
        }

        public bool RefundItem(ShoppingCartItem refundedItem, int ajustement)
        {

            WallyDAL dal = new WallyDAL();
            bool wasItemRefunded = false;

            if(ajustement <= refundedItem.quantity)
            {
                dal.RefundUpdateItemQuantity(DisplayedOrder, refundedItem.ItemId, ajustement);
                refundedItem.quantity -= ajustement;
                OnPropertyChanged("ProductsInOrder");
                wasItemRefunded = true;
            }

            return wasItemRefunded;
        }

        // Handlers
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
