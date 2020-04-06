using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace P04_PLINQ
{
    public class Customer
    {
        public static IEnumerable<string> GetCustomersAsStrings()
        {
            return System.IO.File.ReadAllLines(@"plinqdata.csv")
                                            .SkipWhile((line) => line.StartsWith("CUSTOMERS") == false)
                                             .Skip(1)
                                            .TakeWhile((line) => line.StartsWith("END CUSTOMERS") == false);
        }

        public static IEnumerable<Customer> GetCustomers()
        {
            var customers = System.IO.File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), @"plinqdata.csv"))
                                             .SkipWhile((line) => line.StartsWith("CUSTOMERS") == false)
                                             .Skip(1)
                                             .TakeWhile((line) => line.StartsWith("END CUSTOMERS") == false);
            return (from line in customers
                    let fields = line.Split(',')
                    let custID = fields[0].Trim()
                    select new Customer()
                    {
                        CustomerID = custID,
                        CustomerName = fields[1].Trim(),
                        Address = fields[2].Trim(),
                        City = fields[3].Trim(),
                        PostalCode = fields[4].Trim()
                    });
        }

        public static Order[] GetOrdersForCustomer(string id)
        {
            // Assumes we copied the file correctly!
            var orders = System.IO.File.ReadAllLines(@"plinqdata.csv")
                                             .SkipWhile((line) => line.StartsWith("ORDERS") == false)
                                              .Skip(1)
                                            .TakeWhile((line) => line.StartsWith("END ORDERS") == false);
            var orderStrings = from line in orders
                               let fields = line.Split(',')
                               where fields[1].CompareTo(id) == 0
                               select new Order()
                               {
                                   OrderID = Convert.ToInt32(fields[0]),
                                   CustomerID = fields[1].Trim(),
                                   OrderDate = DateTime.Parse(fields[2], new CultureInfo("en-US", false)),
                                   ShippedDate = DateTime.Parse(fields[3], new CultureInfo("en-US", false))
                               };
            return orderStrings.ToArray();
        }

        private Lazy<Order[]> _orders;
        public Customer()
        {
            _orders = new Lazy<Order[]>(() => GetOrdersForCustomer(CustomerID));
        }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public Order[] Orders {
            get {
                return _orders.Value;
            }
        }

        public override string ToString() {
            return $"Customer: {CustomerID} {CustomerName} {Address} {City} {PostalCode}";
        }
    }

}
