using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P04_PLINQ
{
    public class OrderDetail
    {
        public static IEnumerable<OrderDetail> GetOrderDetails()
        {
            // Assumes we copied the file correctly!
            var orderDetails = System.IO.File.ReadAllLines(@"plinqdata.csv")
                                            .SkipWhile((line) => line.StartsWith("ORDER DETAILS") == false)
                                             .Skip(1)
                                            .TakeWhile((line) => line.StartsWith("END ORDER DETAILS") == false);

            return from line in orderDetails
                   let fields = line.Split(',')
                   select new OrderDetail()
                   {
                       OrderID = Convert.ToInt32(fields[0]),
                       ProductID = Convert.ToInt32(fields[1]),
                       UnitPrice = Convert.ToDouble(fields[2]),
                       Quantity = Convert.ToDouble(fields[3]),
                       Discount = Convert.ToDouble(fields[4])
                   };
        }

        

        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public double UnitPrice { get; set; }
        public double Quantity { get; set; }
        public double Discount { get; set; }
        public override string ToString() {
            return $"OrderDetail: {OrderID} {ProductID} {UnitPrice} {Quantity} {Discount}";
        }
    }
}
