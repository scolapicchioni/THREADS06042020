using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace P04_PLINQ
{
    class Program
    {
        static void Main(string[] args)
        {
            //http://www.albahari.com/threading/part5.aspx#_PLINQ
            //https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/parallel-linq-plinq

            Ex08();
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        private static void Ex08() {
            CancellationTokenSource cts = new CancellationTokenSource();

            int print(int x) {
                Console.WriteLine($"{x} {Thread.CurrentThread.ManagedThreadId} {Task.CurrentId}");
                if (cts.Token.IsCancellationRequested)
                    cts.Token.ThrowIfCancellationRequested();
                return x;
            }

            // Start a new asynchronous task that will cancel the 
            // operation from another thread. Typically you would call
            // Cancel() in response to a button click or some other
            // user interface event.
            Task.Run(() => {
                Console.WriteLine("Press C to Cancel");
                while (Console.ReadKey().Key != ConsoleKey.C) ;
                cts.Cancel();
            });

            int[] results = null;
            try {
                results = Enumerable
                            .Range(1, 10000000)
                            .AsParallel()
                            .WithCancellation(cts.Token)
                            .Where(num=> num % 3 == 0)
                            .OrderByDescending(num => num)
                            .Select(print)
                            .ToArray();
                if (results != null) {
                    foreach (var v in results)
                        Console.WriteLine(v);
                }
                Console.WriteLine();
            } catch (OperationCanceledException e) {
                Console.WriteLine($"{e.GetType().Name}! {e.Message}");
            } catch (AggregateException ae) {
                Console.WriteLine($"{ae.GetType().Name}! {ae.Message}");
                if (ae.InnerExceptions != null) {
                    foreach (Exception e in ae.InnerExceptions)
                        Console.WriteLine($"\t{e.GetType().Name}! {e.Message}");
                }
            } finally {
                cts.Dispose();
            }

        }

        static void Ex07() {
            //catching exceptions thrown during parallel query execution

            int print(int x ) {
                Console.WriteLine($"Selecting Item:{x} - ThreadId {Thread.CurrentThread.ManagedThreadId} - TaskId: {Task.CurrentId}");
                if (x % 100 == 0)
                    throw new ArgumentException($"I don't like this {x}!");
                return x;
            }

            var listOfNumbers = Enumerable.Range(0, 1000);
            var query = listOfNumbers
                        .AsParallel()
                        .AsOrdered()
                        .Select(print);
            
            try {
                foreach (var item in query) {
                    Console.WriteLine($"\tforeach - Item:{item} - ThreadId {Thread.CurrentThread.ManagedThreadId} - TaskId: {Task.CurrentId}");
                }
            } catch (Exception e) { //we get an AggregateException
                Console.WriteLine($"caught {e}");
            }

            try {
                query.ForAll((item) => Console.WriteLine($"ForAll - Item:{item} - ThreadId{Thread.CurrentThread.ManagedThreadId} - TaskId: {Task.CurrentId}"));
            } catch (Exception e) { //we get an AggregateException
                Console.WriteLine($"caught {e}");
            }
        }

        static void Ex06() {
            //You can negate the effect of AsOrdered later in a query by calling AsUnordered: 
            //this introduces a “random shuffle point” which allows the query 
            //to execute more efficiently from that point on.

            var orders = Order.GetOrders();
            var orderDetails = OrderDetail.GetOrderDetails();

            
            var q2 = orders
               .AsParallel()
               .Where(o => {
                   Console.WriteLine($"where - {o} ThreadID: {Thread.CurrentThread.ManagedThreadId}");
                   return o.OrderDate < DateTime.Parse("07/04/1997", new CultureInfo("en-US", false));
               })
               .OrderBy(o => {
                   Console.WriteLine($"orderby - {o} ThreadID: {Thread.CurrentThread.ManagedThreadId}");
                   return o.CustomerID;
               })
               .Take(20) // Preserve original ordering for Take operation.
               .AsUnordered()  // Remove ordering constraint to make join faster.
               .Join(
                      //For query operators that accept two input sequences 
                      //(Join, GroupJoin, Concat, Union, Intersect, Except, and Zip), 
                      //you must apply AsParallel() to both input sequences 
                      orderDetails.AsParallel(),
                      ord => ord.OrderID,
                      od => od.OrderID,
                      (ord, od) => {
                          Console.WriteLine($"join - {ord} - {od} - {Thread.CurrentThread.ManagedThreadId}");
                          return new {
                              ID = ord.OrderID,
                              Customer = ord.CustomerID,
                              Product = od.ProductID
                          };
                      })
                .OrderBy(i => {
                     Console.WriteLine($"second orderby - {i} ThreadID: {Thread.CurrentThread.ManagedThreadId}");
                     return i.Product;
                 }); // Apply new ordering to final result sequence.

            foreach (var v in q2)
                Console.WriteLine($"foreach - {v} ThreadID: {Thread.CurrentThread.ManagedThreadId}");


        }
        static void Ex05() {
            //A side effect of parallelizing the query operators 
            //is that when the results are collated, 
            //it’s not necessarily in the same order that they were submitted. 
            //In other words, LINQ’s normal order-preservation guarantee for sequences no longer holds.
            //
            //If you need order preservation, you can force it by calling AsOrdered() after AsParallel():
            //
            //Calling AsOrdered incurs a performance hit with large numbers of elements 
            //because PLINQ must keep track of each element’s original position.
            int print(int x) {
                Console.WriteLine($"Selecting Item:{x} - ThreadId {Thread.CurrentThread.ManagedThreadId} - TaskId: {Task.CurrentId}");
                return x;
            };

            var listOfNumbers = Enumerable.Range(0, 1000);
            var query = listOfNumbers
                        .AsParallel()
                        .AsOrdered()
                        .Select(print);
            //query.ForAll((item) => Console.WriteLine($"ForAll - Item:{item} - ThreadId{Thread.CurrentThread.ManagedThreadId} - TaskId: {Task.CurrentId}"));
            foreach (var item in query) {
                Console.WriteLine($"\tforeach - Item:{item} - ThreadId {Thread.CurrentThread.ManagedThreadId} - TaskId: {Task.CurrentId}");
            }
        }

        private static void Ex04() {
            int print(int x) {
                Console.WriteLine($"Selecting Item:{x} - ThreadId {Thread.CurrentThread.ManagedThreadId} - TaskId: {Task.CurrentId}");
                return x;
            };

            var listOfNumbers = Enumerable.Range(0, 1000);
            var query = listOfNumbers
                        .AsParallel() //returns a parallel enumerable
                        .Select(print);
            /*
             * One of PLINQ’s advantages is that it conveniently collates the results 
             * from parallelized work into a single output sequence. 
             * Sometimes, though, all that you end up doing with that sequence 
             * is running some function once over each element:
             * 
             * If this is the case — and you don’t care about the order 
             * in which the elements are processed — you can improve 
             * efficiency with PLINQ’s ForAll method.
             * 
             * The ForAll method runs a delegate over every output element of a ParallelQuery. 
             * It hooks right into PLINQ’s internals, bypassing the steps of collating 
             * and enumerating the results.
             */

            query.ForAll((item) => Console.WriteLine($"\tForAll - Item:{item} - ThreadId {Thread.CurrentThread.ManagedThreadId} - TaskId: {Task.CurrentId}"));
        }

        static void Ex03() {
            /*
             * Like ordinary LINQ queries, PLINQ queries are lazily evaluated. 
             * This means that execution is triggered only 
             * when you begin consuming the results — typically 
             * via a foreach loop (although it may also be 
             * via a conversion operator such as ToArray 
             * or an operator that returns a single element or value).
             * 
             * As you enumerate the results, though, 
             * execution proceeds somewhat differently from that 
             * of an ordinary sequential query. 
             * A sequential query is powered entirely by the consumer 
             * in a “pull” fashion: each element from the input sequence 
             * is fetched exactly when required by the consumer. 
             * A parallel query ordinarily uses independent threads 
             * to fetch elements from the input sequence slightly ahead 
             * of when they’re needed by the consumer (rather like a teleprompter 
             * for newsreaders, or an antiskip buffer in CD players). 
             * It then processes the elements in parallel through 
             * the query chain, holding the results in a small buffer 
             * so that they’re ready for the consumer on demand. 
             * 
             * You can tweak PLINQ’s buffering behavior by calling 
             * WithMergeOptions after AsParallel. 
             * The default value of AutoBuffered generally gives the best overall results. 
             * NotBuffered disables the buffer and is useful 
             * if you want to see results as soon as possible; 
             * FullyBuffered caches the entire result set before 
             * presenting it to the consumer (the OrderBy and Reverse 
             * operators naturally work this way, as do the element, 
             * aggregation, and conversion operators).
             */

            int print(int x) {
                Console.WriteLine($"Selecting Item:{x} - ThreadId {Thread.CurrentThread.ManagedThreadId} - TaskId: {Task.CurrentId}");
                return x;
            };

            var listOfNumbers = Enumerable.Range(0, 1000);
            var query = listOfNumbers
                        .AsParallel() //returns a parallel enumerable
                        .WithMergeOptions(ParallelMergeOptions.FullyBuffered)
                        .Select(print);
            foreach (var item in query) {
                Console.WriteLine($"\tforeach - Item:{item} - ThreadId {Thread.CurrentThread.ManagedThreadId} - TaskId: {Task.CurrentId}");
            }
        }

        static void Ex02() {
            int print (int x) {
                Console.WriteLine($"Selecting Item:{x} - ThreadId {Thread.CurrentThread.ManagedThreadId} - TaskId: {Task.CurrentId}");
                return x;
            };

            //The comprehension syntax is also available
            var listOfNumbers = Enumerable.Range(0, 1000);
            var query = from n in listOfNumbers.AsParallel()
                        select print(n);
            foreach (var item in query) {
                Console.WriteLine($"\tforeach - Item:{item} - ThreadId {Thread.CurrentThread.ManagedThreadId} - TaskId: {Task.CurrentId}");
            }
        }

        static void Ex01() {
            //When a set of tasks must be performed on many data values, 
            //we can parallelize by having each thread perform the (same) 
            //set of tasks on a subset of values. 
            //This is called data parallelism 
            //because we are partitioning the data between threads.

            //PLINQ automatically parallelizes local LINQ queries. 
            //PLINQ has the advantage of being easy to use 
            //in that it offloads the burden of both work partitioning 
            //and result collation to the Framework.
            //To use PLINQ, simply call AsParallel() 
            //on the input sequence and then continue the LINQ query as usual

            int print(int x) {
                Console.WriteLine($"Selecting Item:{x} - ThreadId {Thread.CurrentThread.ManagedThreadId} - TaskId: {Task.CurrentId}");
                return x;
            };

            var listOfNumbers = Enumerable.Range(0, 1000);
            var query = listOfNumbers
                        .AsParallel() //returns a parallel enumerable
                        .Select(print);
            //we're back to Thread Id 1
            foreach (var item in query) {
                Console.WriteLine($"\tforeach - Item:{item} - ThreadId {Thread.CurrentThread.ManagedThreadId} - TaskId: {Task.CurrentId}");
            }
        }
    }
}
