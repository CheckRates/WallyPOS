

namespace WallyPOS.Classes.Model
{
    /// <summary>
    /// The Customer class represents the people who shopped at one of Wally's World Branches
    /// </summary>
    public class Customer
    {
        //----------ATTRIBUTES----------//
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNum { get; set; }


        //----------METHODS----------//
        /// <summary>
        /// Default constructor for the Customer Class
        /// </summary>
        public Customer()
        { }

        /// <summary>
        /// Constructor for the Customer Class
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="phoneNum"></param>
        public Customer(string firstName, string lastName, string phoneNum)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNum = phoneNum;
        }
    }
}