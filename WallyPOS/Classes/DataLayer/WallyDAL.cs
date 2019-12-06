using System;
using System.Data;
using System.Collections.Generic;
using WallyPOS.Classes.Model;
using MySql.Data.MySqlClient;


namespace WallyPOS.Classes.DataLayer
{
    public class WallyDAL
    {
        //----------ATTRIBUTES----------//
        private string connectionString = "server=127.0.0.1;user id=root;database=GGWally;password=Conestoga1;SslMode=none";

        //----------METHODS----------//
        /// <summary>
        /// Default Constructor for the Data Access Class of Wally's World
        /// </summary>
        public WallyDAL()
        { }

        /// <summary>
        /// Access the database to get a List of all availible Branches 
        /// </summary>
        /// <returns>Returns a List of Branches</returns>
        public List<Branch> GetBranches()
        {
            const string sqlStatement = @"  SELECT *
                                            FROM Branch                                             
                                            ORDER BY BranchName";

            using (var myConn = new MySqlConnection(connectionString))
            {
                var myCommand = new MySqlCommand(sqlStatement, myConn);

                //For offline connection we will use  MySqlDataAdapter class.  
                var myAdapter = new MySqlDataAdapter
                {
                    SelectCommand = myCommand
                };

                var dataTable = new DataTable();
                myAdapter.Fill(dataTable);

                var branches = DataTableToBranchList(dataTable);

                return branches;
            }
        }

        /// <summary>
        /// Access the database to get a List of all Customers that meet criterias set in the parameters
        /// </summary>
        /// <returns>Returns the List of Customers that were filtered through the parameters</returns>
        public List<Customer> GetCustomers(string firstName, string lastName, string phoneNumber)
        {
            const string sqlStatement = @"SELECT * 
                                                  FROM customer
                                                  WHERE
                                                  (FirstName LIKE CONCAT(@FNAME, '%') OR @FNAME= '') AND
                                                  (LastName LIKE CONCAT(@LNAME, '%') OR @LNAME  = '') AND  
                                                  (PhoneNum LIKE CONCAT(@PNUMBER, '%') OR @PNUMBER = ''); "; 


            using (var myConn = new MySqlConnection(connectionString))
            {
                var myCommand = new MySqlCommand(sqlStatement, myConn);
                myCommand.Parameters.AddWithValue("@FNAME", firstName);
                myCommand.Parameters.AddWithValue("@LNAME", lastName);
                myCommand.Parameters.AddWithValue("@PNUMBER", phoneNumber);

                //For offline connection we will use  MySqlDataAdapter class.  
                var myAdapter = new MySqlDataAdapter
                {
                    SelectCommand = myCommand
                };

                var dataTable = new DataTable();
                myAdapter.Fill(dataTable);

                var filteredCustomers= DataTableToCustomerList(dataTable);

                return filteredCustomers;
            }
        }

        /// <summary>
        /// Inserts a new customer into the database
        /// </summary>
        /// <param name="newCustomer"> <b>Customer</b> - Object representing the customer that will be added to the database></param>
        public void CreateCustomer(Customer newCustomer)
        {
            // Convert phone Number into correct format
            newCustomer.PhoneNum = string.Format("{0:###-###-####}", Convert.ToInt64(newCustomer.PhoneNum));

            using (var myConn = new MySqlConnection(connectionString))
            {
                const string sqlStatement = @"  INSERT INTO customer (FirstName, LastName, PhoneNum)
	                                            VALUES (@FIRSTNAME, @LASTNAME, @PHONENUM); ";

                var myCommand = new MySqlCommand(sqlStatement, myConn);

                myCommand.Parameters.AddWithValue("@FIRSTNAME", newCustomer.FirstName);
                myCommand.Parameters.AddWithValue("@LASTNAME", newCustomer.LastName);
                myCommand.Parameters.AddWithValue("@PHONENUM", newCustomer.PhoneNum);

                myConn.Open();
                myCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Access the database to get a List of all Products that meet criterias set in the parameters
        /// </summary>
        /// <param name="productName"> <b>string</b> - product name string></param>
        /// <returns> Returns a List of Items that meetthe filtering criteria</returns>
        public List<Item> GetProductsByName(string productName)
        {
            const string sqlStatement = @"SELECT * 
                                                  FROM item
                                                  WHERE
                                                  (ItemName LIKE CONCAT(@PRODUCTNAME, '%') OR @PRODUCTNAME= '') ";

            using (var myConn = new MySqlConnection(connectionString))
            {
                var myCommand = new MySqlCommand(sqlStatement, myConn);
                myCommand.Parameters.AddWithValue("@PRODUCTNAME", productName);

                //For offline connection we will use MySqlDataAdapter class.  
                var myAdapter = new MySqlDataAdapter
                {
                    SelectCommand = myCommand
                };

                var dataTable = new DataTable();
                myAdapter.Fill(dataTable);

                var products = DataTableToItemList(dataTable);

                return products;
            }
        }

        /// <summary>
        /// Inserts an new order into Wally's Database
        /// </summary>
        /// <param name="order"> <b>Order</b> - Order object that will be inserted into the database></param>
        /// <returns> Returns the order id of the newly inserted order</returns>
        public int CreateNewOrder(Order order)
        {
            using (var myConn = new MySqlConnection(connectionString))
            {
                const string sqlStatement = @"  INSERT INTO invoice (CustomerId, BranchID, OrderDate, PaymentAuthStatus)
	                                            VALUES (@CUSTOMERID, @BRANCHID, @DATE, @PAYSTATUS); ";

                var myCommand = new MySqlCommand(sqlStatement, myConn);

                myCommand.Parameters.AddWithValue("@CUSTOMERID", order.CustomerId);
                myCommand.Parameters.AddWithValue("@BRANCHID", order.BranchId);
                myCommand.Parameters.AddWithValue("@DATE", order.OrderDate);
                myCommand.Parameters.AddWithValue("@PAYSTATUS", true); 

                myConn.Open();

                myCommand.ExecuteNonQuery();
            }

            return GetLastPrimaryKey();
        }

        /// <summary>
        /// Gets the last inserted primary key 
        /// </summary>
        /// <returns> Returns the last inserted primary key</returns>
        public int GetLastPrimaryKey()
        {
            Int32 lastOrderId = -1;

            using (var myConn = new MySqlConnection(connectionString))
            {
                const string sqlStatement = @"  SELECT LAST_INSERT_ID(); ";

                var myCommand = new MySqlCommand(sqlStatement, myConn);
                myConn.Open();

                lastOrderId = Convert.ToInt32(myCommand.ExecuteScalar());
            }

            return lastOrderId;
        }

        /// <summary>
        /// Connects the order to products by inserting the orderline associative entity
        /// </summary>
        /// <param name="orderId"> <b>int</b> - Order primary key that the products will be connected to ></param>
        /// <param name="shoppingCart"> <b>List<ShoppingCartItem></b> - All the products from a particular order ></param>
        public void InsertOrderLine(int orderId, List<ShoppingCartItem> shoppingCart)
        {
            using (var myConn = new MySqlConnection(connectionString))
            {
                myConn.Open();

                foreach (var product in shoppingCart)
                {
                    const string sqlStatement = @"  INSERT INTO orderline (OrderId, ItemId, Quantity)
	                                                VALUES (@ORDERID, @ITEMID, @QUANTITY); 
                                                    UPDATE item                                
                                                    SET stock = (stock - @QUANTITY) 
                                                        WHERE ItemId = @ITEMID; ";

                    var myCommand = new MySqlCommand(sqlStatement, myConn);

                    myCommand.Parameters.AddWithValue("@ORDERID", orderId);
                    myCommand.Parameters.AddWithValue("@ITEMID", product.ItemId);
                    myCommand.Parameters.AddWithValue("@QUANTITY", product.quantity);

                    myCommand.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Gets all the Orders and their related customer that meet the criteria of the filters 
        /// </summary>
        /// <param name="firstName"> <b>string</b> - First Name field filter criteria</param>
        /// <param name="lastName">  <b>string</b> - Last Name field filter criteria</param>
        /// <param name="phoneNumber"> <b>string</b> - Phone Number field filter criteria</param>
        /// <param name="branchName">  <b>string</b> - Branch Name field filter criteria</param>
        /// <returns></returns>
        public List<CustomerOrder> GetOrders(string firstName, string lastName, string phoneNumber, string branchName)
        {
            const string sqlStatement = @" SELECT * 
                                                   FROM CustomerOrder
                                                   WHERE
		                                           (FirstName LIKE CONCAT(@FNAME, '%') OR @FNAME= '') AND
                                                   (LastName LIKE CONCAT(@LNAME, '%') OR @LNAME  = '') AND  
                                                   (PhoneNum LIKE CONCAT(@PNUMBER, '%') OR @PNUMBER = '') AND
                                                   (BranchName LIKE CONCAT(@BRANCHNAME, '%') OR @BRANCHNAME = ''); ";

            using (var myConn = new MySqlConnection(connectionString))
            {
                var myCommand = new MySqlCommand(sqlStatement, myConn);
                myCommand.Parameters.AddWithValue("@FNAME", firstName);
                myCommand.Parameters.AddWithValue("@LNAME", lastName);
                myCommand.Parameters.AddWithValue("@PNUMBER", phoneNumber);
                myCommand.Parameters.AddWithValue("@BRANCHNAME", branchName);


                //For offline connection we will use  MySqlDataAdapter class.  
                var myAdapter = new MySqlDataAdapter
                {
                    SelectCommand = myCommand
                };

                var dataTable = new DataTable();
                myAdapter.Fill(dataTable);

                var filteredCustomersOrders = DataTableToCustomerOrderList(dataTable);

                return filteredCustomersOrders;
            }
        }

        /// <summary>
        /// Gets a single order with the related customer by searching by the primary key
        /// </summary>
        /// <param name="orderId"> <b>int</b> - Order primary key that will be searched </param>
        /// <returns> Returns one instance </returns>
        public CustomerOrder GetCustomerOrder(int orderId)
        {
            const string sqlStatement = @" SELECT * 
                                                   FROM CustomerOrder
                                                   WHERE
		                                           OrderId = @ORDERID; ";

            using (var myConn = new MySqlConnection(connectionString))
            {
                var myCommand = new MySqlCommand(sqlStatement, myConn);
                myCommand.Parameters.AddWithValue("@ORDERID", orderId);

                //For offline connection we will use  MySqlDataAdapter class.  
                var myAdapter = new MySqlDataAdapter
                {
                    SelectCommand = myCommand
                };

                var dataTable = new DataTable();
                myAdapter.Fill(dataTable);

                var filteredCustomersOrders = DataTableToCustomerOrderList(dataTable);

                return filteredCustomersOrders[0];
            }
        }

        public List<ShoppingCartItem> GetProductsFromOrder(int orderId)
        {
            const string sqlStatement = @" SELECT ol.ItemId, ItemName, Quantity  
                                           FROM invoice AS o
	                                            INNER JOIN orderline AS ol ON o.OrderId = ol.OrderId
                                                INNER JOIN item AS p ON ol.ItemId = p.ItemId
                                                WHERE o.OrderId = @ORDERID;";


            using (var myConn = new MySqlConnection(connectionString))
            {
                var myCommand = new MySqlCommand(sqlStatement, myConn);
                myCommand.Parameters.AddWithValue("@ORDERID", orderId);

                //For offline connection we will use  MySqlDataAdapter class.  
                var myAdapter = new MySqlDataAdapter
                {
                    SelectCommand = myCommand
                };

                var dataTable = new DataTable();
                myAdapter.Fill(dataTable);

                var orderProducts = DataTableToCartItemsList(dataTable);

                return orderProducts;
            }
        }

        public void RefundUpdateItemQuantity(int orderId, int refundedItemId, int ajustment)
        {
            const string sqlStatement = @"  UPDATE orderline
                                            SET 
                                            quantity = (quantity - @AJUSTMENT)
                                            WHERE OrderId = @ORDERID AND ItemId = @ITEMID;
                                            UPDATE invoice
                                            SET
                                            PaymentAuthStatus = false
                                            WHERE OrderID = @ORDERID;
                                            UPDATE item
                                            SET
                                            stock = (stock + @AJUSTMENT)
                                            WHERE ItemId = @ITEMID; ";

            using (var myConn = new MySqlConnection(connectionString))
            {
                var myCommand = new MySqlCommand(sqlStatement, myConn);
                myCommand.Parameters.AddWithValue("@AJUSTMENT", ajustment);
                myCommand.Parameters.AddWithValue("@ORDERID", orderId);
                myCommand.Parameters.AddWithValue("@ITEMID", refundedItemId);

                myConn.Open();

                myCommand.ExecuteNonQuery();
            }
        }

        //---------> Data Conversions
        /// <summary>
        /// Converts a the resulted datatable of a query to a List object with the proper data conversions 
        /// </summary>
        /// <param name="table" <b>DataTable</b> - Object that will be converted to a valid List></param>
        /// <returns>Returns the appropriate List of Item objects</returns>
        private List<Item> DataTableToItemList(DataTable table)
        {
            var products = new List<Item>();

            foreach (DataRow row in table.Rows)
            {
                products.Add(new Item
                {
                    ItemId = Convert.ToInt32(row["ItemId"]),
                    ItemName = row["ItemName"].ToString(),
                    UnitPrice = (float)Convert.ToDouble(row["UnitPrice"]),
                    ItemStock = Convert.ToInt32(row["Stock"]),
                    ItemDescp = row["ItemDescp"].ToString(),
                    Colour = row["Colour"].ToString(),
                    Size = row["Size"].ToString(),
                    Pattern = row["Pattern"].ToString(), 
                    ItemType = row["ITemType"].ToString()
                });
            }

            return products;
        }


        /// <summary>
        /// Converts a the resulted datatable of a query to a List object with the proper data conversions 
        /// </summary>
        /// <param name="table"<b>DataTable</b> - Object that will be converted to a valid List></param>
        /// <returns>Returns the appropriate List of Branch objects</returns>
        private List<Branch> DataTableToBranchList(DataTable table)
        {
            var branches = new List<Branch>();

            foreach (DataRow row in table.Rows)
            {
                branches.Add(new Branch
                {
                    BranchId = Convert.ToInt32(row["BranchId"]),
                    BranchName = row["BranchName"].ToString(),
                });
            }

            return branches;
        }

        /// <summary>
        /// Converts the resulted datatable of a query to  a List object with the propre data conversions
        /// </summary>
        /// <param name="table"<b>DataTable</b> - Object that will be converted to a valid List ></param>
        /// <returns>Returns the appropriate List of Customer objects</returns>
        private List<Customer> DataTableToCustomerList(DataTable table)
        {
            var customers = new List<Customer>();

            foreach (DataRow row in table.Rows)
            {
                customers.Add(new Customer
                {
                    CustomerId = Convert.ToInt32(row["CustomerId"]),
                    FirstName = row["FirstName"].ToString(),
                    LastName = row["LastName"].ToString(),
                    PhoneNum = row["PhoneNum"].ToString()
                });
            }

            return customers;
        }

        private List<CustomerOrder> DataTableToCustomerOrderList(DataTable table)
        {
            var productsList = new List<CustomerOrder>();

            foreach (DataRow row in table.Rows)
            {
                CustomerOrder newCustomerOrder = new CustomerOrder();
                newCustomerOrder.order.OrderId = Convert.ToInt32(row["OrderId"]);
                newCustomerOrder.customer.FirstName = row["FirstName"].ToString();
                newCustomerOrder.customer.LastName = row["LastName"].ToString();
                newCustomerOrder.customer.PhoneNum = row["PhoneNum"].ToString();
                newCustomerOrder.branch.BranchName = row["BranchName"].ToString();
                newCustomerOrder.order.OrderDate = Convert.ToDateTime(row["OrderDate"]);
                newCustomerOrder.order.PaymentAuthStatus = Convert.ToBoolean(row["PaymentAuthStatus"]);

                productsList.Add(newCustomerOrder);
            }

            return productsList;
        }

        private List<ShoppingCartItem> DataTableToCartItemsList(DataTable table)
        {
            var cartItems = new List<ShoppingCartItem>();

            foreach (DataRow row in table.Rows)
            {
                cartItems.Add(new ShoppingCartItem
                {
                    ItemId = Convert.ToInt32(row["ItemId"]),
                    ItemName = row["ItemName"].ToString(),
                    quantity = Convert.ToInt32(row["Quantity"])
                });
            }

            return cartItems;
        }
    }
}