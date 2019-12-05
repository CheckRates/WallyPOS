
namespace WallyPOS.Classes.Model
{
    /// <summary>
    /// A Branch object represent one of the possible locations that a Wally's customer can shop
    /// </summary>
    public class Branch
    {
        //----------ATTRIBUTES----------//
        public int BranchId { get; set; }
        public string BranchName { get; set; }

        //----------MEHTODS----------//
        /// <summary>
        /// Default constructor for the Branch class
        /// </summary>
        public Branch()
        { }
    }
}
