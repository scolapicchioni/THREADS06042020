using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P04_PLINQ
{
    public class Product
    {
        public static IEnumerable<Product> GetProducts()
        {
            // Assumes we copied the file correctly!
            var products = System.IO.File.ReadAllLines(@"plinqdata.csv")
                                            .SkipWhile((line) => line.StartsWith("PRODUCTS") == false)
                                             .Skip(1)
                                            .TakeWhile((line) => line.StartsWith("END PRODUCTS") == false);
            return from line in products
                   let fields = line.Split(',')
                   select new Product()
                   {
                       ProductID = Convert.ToInt32(fields[0]),
                       ProductName = fields[1].Trim(),
                       UnitPrice = Convert.ToDouble(fields[2])

                   };
        }

        public string ProductName { get; set; }
        public int ProductID { get; set; }
        public double UnitPrice { get; set; }
        public override string ToString() {
            return $"Products: {ProductID} {ProductName} {UnitPrice}";
        }
    }
}
