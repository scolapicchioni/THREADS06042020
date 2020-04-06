using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace P04_PLINQ
{
    public class Order
    {
        //  "10248, VINET, 7/4/1996 12:00:00 AM, 7/16/1996 12:00:00 AM
        public static IEnumerable<Order> GetOrders()
        {
            // Assumes we copied the file correctly!
            var orders = System.IO.File.ReadAllLines(@"plinqdata.csv")
                                            .SkipWhile((line) => line.StartsWith("ORDERS") == false)
                                             .Skip(1)
                                            .TakeWhile((line) => line.StartsWith("END ORDERS") == false);
            return from line in orders
                   let fields = line.Split(',')

                   select new Order()
                   {
                       OrderID = Convert.ToInt32(fields[0]),
                       CustomerID = fields[1].Trim(),
                       OrderDate = DateTime.Parse(fields[2], new CultureInfo("en-US", false)),
                       ShippedDate = DateTime.Parse(fields[3], new CultureInfo("en-US", false))
                   };
        }

        public static OrderDetail[] GetOrderDetailsForOrder(int id)
        {
            // Assumes we copied the file correctly!
            var orderDetails = System.IO.File.ReadAllLines(@"plinqdata.csv")
                                            .SkipWhile((line) => line.StartsWith("ORDER DETAILS") == false)
                                             .Skip(1)
                                            .TakeWhile((line) => line.StartsWith("END ORDER DETAILS") == false);

            var orderDetailStrings = from line in orderDetails
                                     let fields = line.Split(',')
                                     let ordID = Convert.ToInt32(fields[0])
                                     where ordID == id
                                     select new OrderDetail()
                                     {
                                         OrderID = ordID,
                                         ProductID = Convert.ToInt32(fields[1]),
                                         UnitPrice = Convert.ToDouble(fields[2]),
                                         Quantity = Convert.ToDouble(fields[3]),
                                         Discount = Convert.ToDouble(fields[4])
                                     };

            return orderDetailStrings.ToArray();
        }

        private Lazy<OrderDetail[]> _orderDetails;
        public Order()
        {
            _orderDetails = new Lazy<OrderDetail[]>(() => GetOrderDetailsForOrder(OrderID));
        }
        public int OrderID { get; set; }
        public string CustomerID { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public OrderDetail[] OrderDetails { get { return _orderDetails.Value; } }

        public override string ToString() {
            return $"Order: {OrderID} {CustomerID} {OrderDate} {ShippedDate}";
        }
    }
}
