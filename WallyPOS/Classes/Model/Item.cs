
namespace WallyPOS.Classes.Model
{
    /// <summary>
    /// Represents the avalible products of Wally's World Wallpaper Store 
    /// </summary>
    public class Item
    {
        //----------ATTRIBUTES----------//
        public int ItemId {get; set;}
        public string ItemName { get; set; }
        public float UnitPrice { get; set; }
        public string ItemDescp { get; set; }
        public int ItemStock { get; set; }
        public string Colour { get; set; }
        public string Size { get; set; }
        public string Pattern { get; set; }
        public string ItemType { get; set; }

        //----------METHODS----------//
        /// <summary>
        /// Default constructor for the Item (Product)
        /// </summary>
        public Item()
        { }

        /// <summary>
        /// Copy constructor for the Item (Product)
        /// </summary>
        /// <param name="copyItem"></param>
        public Item(Item copyItem)
        {
            ItemId = copyItem.ItemId;
            ItemName = copyItem.ItemName;
            UnitPrice = copyItem.UnitPrice;
            ItemDescp = copyItem.ItemDescp;
            ItemStock = copyItem.ItemStock;
            Colour = copyItem.Colour;
            Size = copyItem.Size;
            Pattern = copyItem.Pattern;
            ItemType = copyItem.ItemType;
        }
    }
}