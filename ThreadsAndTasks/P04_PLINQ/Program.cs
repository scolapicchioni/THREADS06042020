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

            Ex01();
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
            ParallelQuery<int> query = from n in listOfNumbers.AsParallel()
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

            var listOfNumbers = Enumerable.Range(0, 100);
            ParallelQuery<int> query = listOfNumbers
                        .AsParallel() //returns a parallel enumerable
                        .Select(print);
            //we're back to Thread Id 1
            foreach (var item in query) {
                Console.WriteLine($"\tforeach - Item:{item} - ThreadId {Thread.CurrentThread.ManagedThreadId} - TaskId: {Task.CurrentId}");
            }

            /*
             * Selecting Item:3 - ThreadId 6 - TaskId: 4
Selecting Item:2 - ThreadId 5 - TaskId: 5
Selecting Item:9 - ThreadId 5 - TaskId: 5
Selecting Item:10 - ThreadId 5 - TaskId: 5
Selecting Item:11 - ThreadId 5 - TaskId: 5
Selecting Item:12 - ThreadId 5 - TaskId: 5
Selecting Item:13 - ThreadId 5 - TaskId: 5
Selecting Item:14 - ThreadId 5 - TaskId: 5
Selecting Item:15 - ThreadId 5 - TaskId: 5
Selecting Item:16 - ThreadId 5 - TaskId: 5
Selecting Item:17 - ThreadId 5 - TaskId: 5
Selecting Item:18 - ThreadId 5 - TaskId: 5
Selecting Item:19 - ThreadId 5 - TaskId: 5
Selecting Item:20 - ThreadId 5 - TaskId: 5
Selecting Item:21 - ThreadId 5 - TaskId: 5
Selecting Item:22 - ThreadId 5 - TaskId: 5
Selecting Item:23 - ThreadId 5 - TaskId: 5
Selecting Item:24 - ThreadId 5 - TaskId: 5
Selecting Item:25 - ThreadId 5 - TaskId: 5
Selecting Item:26 - ThreadId 5 - TaskId: 5
Selecting Item:27 - ThreadId 5 - TaskId: 5
Selecting Item:28 - ThreadId 5 - TaskId: 5
Selecting Item:29 - ThreadId 5 - TaskId: 5
Selecting Item:30 - ThreadId 5 - TaskId: 5
Selecting Item:31 - ThreadId 5 - TaskId: 5
Selecting Item:32 - ThreadId 5 - TaskId: 5
Selecting Item:33 - ThreadId 5 - TaskId: 5
Selecting Item:34 - ThreadId 5 - TaskId: 5
Selecting Item:35 - ThreadId 5 - TaskId: 5
Selecting Item:36 - ThreadId 5 - TaskId: 5
Selecting Item:37 - ThreadId 5 - TaskId: 5
Selecting Item:38 - ThreadId 5 - TaskId: 5
Selecting Item:39 - ThreadId 5 - TaskId: 5
Selecting Item:40 - ThreadId 5 - TaskId: 5
Selecting Item:41 - ThreadId 5 - TaskId: 5
Selecting Item:42 - ThreadId 5 - TaskId: 5
Selecting Item:43 - ThreadId 5 - TaskId: 5
Selecting Item:44 - ThreadId 5 - TaskId: 5
Selecting Item:0 - ThreadId 10 - TaskId: 8
Selecting Item:48 - ThreadId 10 - TaskId: 8
Selecting Item:49 - ThreadId 10 - TaskId: 8
Selecting Item:50 - ThreadId 10 - TaskId: 8
Selecting Item:51 - ThreadId 10 - TaskId: 8
Selecting Item:52 - ThreadId 10 - TaskId: 8
Selecting Item:1 - ThreadId 7 - TaskId: 9
Selecting Item:54 - ThreadId 7 - TaskId: 9
Selecting Item:55 - ThreadId 7 - TaskId: 9
Selecting Item:56 - ThreadId 7 - TaskId: 9
Selecting Item:57 - ThreadId 7 - TaskId: 9
Selecting Item:58 - ThreadId 7 - TaskId: 9
Selecting Item:59 - ThreadId 7 - TaskId: 9
Selecting Item:60 - ThreadId 7 - TaskId: 9
Selecting Item:61 - ThreadId 7 - TaskId: 9
Selecting Item:62 - ThreadId 7 - TaskId: 9
Selecting Item:63 - ThreadId 7 - TaskId: 9
Selecting Item:64 - ThreadId 7 - TaskId: 9
Selecting Item:65 - ThreadId 7 - TaskId: 9
Selecting Item:8 - ThreadId 6 - TaskId: 4
Selecting Item:67 - ThreadId 6 - TaskId: 4
Selecting Item:68 - ThreadId 6 - TaskId: 4
Selecting Item:69 - ThreadId 6 - TaskId: 4
Selecting Item:70 - ThreadId 6 - TaskId: 4
Selecting Item:71 - ThreadId 6 - TaskId: 4
Selecting Item:72 - ThreadId 6 - TaskId: 4
Selecting Item:73 - ThreadId 6 - TaskId: 4
Selecting Item:74 - ThreadId 6 - TaskId: 4
Selecting Item:75 - ThreadId 6 - TaskId: 4
Selecting Item:76 - ThreadId 6 - TaskId: 4
Selecting Item:77 - ThreadId 6 - TaskId: 4
Selecting Item:78 - ThreadId 6 - TaskId: 4
Selecting Item:7 - ThreadId 8 - TaskId: 6
Selecting Item:81 - ThreadId 8 - TaskId: 6
Selecting Item:82 - ThreadId 8 - TaskId: 6
Selecting Item:83 - ThreadId 8 - TaskId: 6
Selecting Item:84 - ThreadId 8 - TaskId: 6
Selecting Item:85 - ThreadId 8 - TaskId: 6
Selecting Item:86 - ThreadId 8 - TaskId: 6
Selecting Item:87 - ThreadId 8 - TaskId: 6
Selecting Item:88 - ThreadId 8 - TaskId: 6
Selecting Item:89 - ThreadId 8 - TaskId: 6
Selecting Item:90 - ThreadId 8 - TaskId: 6
Selecting Item:91 - ThreadId 8 - TaskId: 6
Selecting Item:92 - ThreadId 8 - TaskId: 6
Selecting Item:93 - ThreadId 8 - TaskId: 6
Selecting Item:94 - ThreadId 8 - TaskId: 6
Selecting Item:66 - ThreadId 7 - TaskId: 9
Selecting Item:53 - ThreadId 10 - TaskId: 8
Selecting Item:45 - ThreadId 5 - TaskId: 5
Selecting Item:4 - ThreadId 4 - TaskId: 2
Selecting Item:99 - ThreadId 4 - TaskId: 2
Selecting Item:6 - ThreadId 3 - TaskId: 3
Selecting Item:79 - ThreadId 6 - TaskId: 4
Selecting Item:95 - ThreadId 8 - TaskId: 6
Selecting Item:96 - ThreadId 7 - TaskId: 9
Selecting Item:97 - ThreadId 7 - TaskId: 9
Selecting Item:46 - ThreadId 5 - TaskId: 5
Selecting Item:47 - ThreadId 5 - TaskId: 5
        foreach - Item:6 - ThreadId 1 - TaskId:
Selecting Item:80 - ThreadId 6 - TaskId: 4
Selecting Item:98 - ThreadId 10 - TaskId: 8
Selecting Item:5 - ThreadId 9 - TaskId: 7
        foreach - Item:2 - ThreadId 1 - TaskId:
        foreach - Item:7 - ThreadId 1 - TaskId:
        foreach - Item:5 - ThreadId 1 - TaskId:
        foreach - Item:0 - ThreadId 1 - TaskId:
        foreach - Item:1 - ThreadId 1 - TaskId:
        foreach - Item:4 - ThreadId 1 - TaskId:
        foreach - Item:3 - ThreadId 1 - TaskId:
        foreach - Item:9 - ThreadId 1 - TaskId:
        foreach - Item:81 - ThreadId 1 - TaskId:
        foreach - Item:48 - ThreadId 1 - TaskId:
        foreach - Item:54 - ThreadId 1 - TaskId:
        foreach - Item:99 - ThreadId 1 - TaskId:
        foreach - Item:8 - ThreadId 1 - TaskId:
        foreach - Item:10 - ThreadId 1 - TaskId:
        foreach - Item:82 - ThreadId 1 - TaskId:
        foreach - Item:49 - ThreadId 1 - TaskId:
        foreach - Item:55 - ThreadId 1 - TaskId:
        foreach - Item:67 - ThreadId 1 - TaskId:
        foreach - Item:11 - ThreadId 1 - TaskId:
        foreach - Item:83 - ThreadId 1 - TaskId:
        foreach - Item:50 - ThreadId 1 - TaskId:
        foreach - Item:56 - ThreadId 1 - TaskId:
        foreach - Item:68 - ThreadId 1 - TaskId:
        foreach - Item:12 - ThreadId 1 - TaskId:
        foreach - Item:84 - ThreadId 1 - TaskId:
        foreach - Item:51 - ThreadId 1 - TaskId:
        foreach - Item:57 - ThreadId 1 - TaskId:
        foreach - Item:69 - ThreadId 1 - TaskId:
        foreach - Item:13 - ThreadId 1 - TaskId:
        foreach - Item:85 - ThreadId 1 - TaskId:
        foreach - Item:52 - ThreadId 1 - TaskId:
        foreach - Item:58 - ThreadId 1 - TaskId:
        foreach - Item:70 - ThreadId 1 - TaskId:
        foreach - Item:14 - ThreadId 1 - TaskId:
        foreach - Item:86 - ThreadId 1 - TaskId:
        foreach - Item:53 - ThreadId 1 - TaskId:
        foreach - Item:59 - ThreadId 1 - TaskId:
        foreach - Item:71 - ThreadId 1 - TaskId:
        foreach - Item:15 - ThreadId 1 - TaskId:
        foreach - Item:87 - ThreadId 1 - TaskId:
        foreach - Item:98 - ThreadId 1 - TaskId:
        foreach - Item:60 - ThreadId 1 - TaskId:
        foreach - Item:72 - ThreadId 1 - TaskId:
        foreach - Item:16 - ThreadId 1 - TaskId:
        foreach - Item:88 - ThreadId 1 - TaskId:
        foreach - Item:61 - ThreadId 1 - TaskId:
        foreach - Item:73 - ThreadId 1 - TaskId:
        foreach - Item:17 - ThreadId 1 - TaskId:
        foreach - Item:89 - ThreadId 1 - TaskId:
        foreach - Item:62 - ThreadId 1 - TaskId:
        foreach - Item:74 - ThreadId 1 - TaskId:
        foreach - Item:18 - ThreadId 1 - TaskId:
        foreach - Item:90 - ThreadId 1 - TaskId:
        foreach - Item:63 - ThreadId 1 - TaskId:
        foreach - Item:75 - ThreadId 1 - TaskId:
        foreach - Item:19 - ThreadId 1 - TaskId:
        foreach - Item:91 - ThreadId 1 - TaskId:
        foreach - Item:64 - ThreadId 1 - TaskId:
        foreach - Item:76 - ThreadId 1 - TaskId:
        foreach - Item:20 - ThreadId 1 - TaskId:
        foreach - Item:92 - ThreadId 1 - TaskId:
        foreach - Item:65 - ThreadId 1 - TaskId:
        foreach - Item:77 - ThreadId 1 - TaskId:
        foreach - Item:21 - ThreadId 1 - TaskId:
        foreach - Item:93 - ThreadId 1 - TaskId:
        foreach - Item:66 - ThreadId 1 - TaskId:
        foreach - Item:78 - ThreadId 1 - TaskId:
        foreach - Item:22 - ThreadId 1 - TaskId:
        foreach - Item:94 - ThreadId 1 - TaskId:
        foreach - Item:96 - ThreadId 1 - TaskId:
        foreach - Item:79 - ThreadId 1 - TaskId:
        foreach - Item:23 - ThreadId 1 - TaskId:
        foreach - Item:95 - ThreadId 1 - TaskId:
        foreach - Item:97 - ThreadId 1 - TaskId:
        foreach - Item:80 - ThreadId 1 - TaskId:
        foreach - Item:24 - ThreadId 1 - TaskId:
        foreach - Item:25 - ThreadId 1 - TaskId:
        foreach - Item:26 - ThreadId 1 - TaskId:
        foreach - Item:27 - ThreadId 1 - TaskId:
        foreach - Item:28 - ThreadId 1 - TaskId:
        foreach - Item:29 - ThreadId 1 - TaskId:
        foreach - Item:30 - ThreadId 1 - TaskId:
        foreach - Item:31 - ThreadId 1 - TaskId:
        foreach - Item:32 - ThreadId 1 - TaskId:
        foreach - Item:33 - ThreadId 1 - TaskId:
        foreach - Item:34 - ThreadId 1 - TaskId:
        foreach - Item:35 - ThreadId 1 - TaskId:
        foreach - Item:36 - ThreadId 1 - TaskId:
        foreach - Item:37 - ThreadId 1 - TaskId:
        foreach - Item:38 - ThreadId 1 - TaskId:
        foreach - Item:39 - ThreadId 1 - TaskId:
        foreach - Item:40 - ThreadId 1 - TaskId:
        foreach - Item:41 - ThreadId 1 - TaskId:
        foreach - Item:42 - ThreadId 1 - TaskId:
        foreach - Item:43 - ThreadId 1 - TaskId:
        foreach - Item:44 - ThreadId 1 - TaskId:
        foreach - Item:45 - ThreadId 1 - TaskId:
        foreach - Item:46 - ThreadId 1 - TaskId:
        foreach - Item:47 - ThreadId 1 - TaskId:*/
        }
    }
}
