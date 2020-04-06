using System;
using System.Linq;
using System.Threading;

namespace ThredPoolExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            Ex07();
            Console.WriteLine("Press Enter to quit...");
            Console.ReadLine();
        }

        static void Ex07() {
            //Timers use the ThreadPool indirectly
            void runThis(object _){
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} {Thread.CurrentThread.IsThreadPoolThread} Press E to End Timer");
            };

            using (Timer timer = new Timer(runThis, null, 3000, 1000)) {
                Console.WriteLine(".... Wait for it...");
                do {
                } while (Console.ReadKey().Key != ConsoleKey.E);
            }

            //Timer timer = null;
            //try {
            //    timer = new Timer(runThis, null, 3000, 1000)) {
            //        Console.WriteLine(".... Wait for it...");
            //        do {
            //        } while (Console.ReadKey().Key != ConsoleKey.E);

            //    }
            //}finally{
            //    timer.Dispose();    
            //}

        }

        static void Ex06() {
            //but you can use the state passed as a parameter
            ThreadLocal<int> threadLocal = new ThreadLocal<int>(() => 3);

            for (int i = 0; i < 100; i++) {
                ThreadPool.QueueUserWorkItem(x => {
                    int tid = Thread.CurrentThread.ManagedThreadId;
                    string tabs = new string(Enumerable.Repeat('\t', tid).ToArray());
                    Thread.Sleep(100);
                    Console.WriteLine($"{tabs}{tid} - BEFORE x == {x}");
                    x = 1000;
                    Thread.Sleep(100);
                    Console.WriteLine($"{tabs}{tid} - AFTER x == {x}");
                }, i);
            }
        }

        static void Ex05() {
            //you cannot really use ThreadLocal, seen the fact that many methods
            //could run on the same Thread...
            ThreadLocal<int> threadLocal = new ThreadLocal<int>(() => 3);

            for (int i = 0; i < 100; i++) {
                ThreadPool.QueueUserWorkItem(x => {
                    int tid = Thread.CurrentThread.ManagedThreadId;
                    string tabs = new string(Enumerable.Repeat('\t', tid).ToArray());
                    Thread.Sleep(100);
                    Console.WriteLine($"{tabs}{tid} - BEFORE threadLocal.Value == {threadLocal.Value}");
                    threadLocal.Value = (int)x;
                    Thread.Sleep(100);
                    Console.WriteLine($"{tabs}{tid} - AFTER threadLocal.Value == {threadLocal.Value}");
                }, i);
            }
        }

        static void Ex04() {
            //this example is equivalent to Ex22
            //but it uses the ThreadPool instead of spawning a new Thread

            CancellationTokenSource cts = new CancellationTokenSource();

            ThreadPool.QueueUserWorkItem(o => {
                CancellationToken ct = (CancellationToken)o;
                Console.Clear();
                Console.WriteLine("Type A to abort");
                try {
                    int i = 0;
                    while (true) {
                        Console.CursorTop = 3;
                        Console.CursorLeft = 3;
                        Console.Write(i++);
                        ct.ThrowIfCancellationRequested();
                    }

                } catch {
                    Console.WriteLine("kthxbye");
                    //undo stuff or just leave...
                }

            }, cts.Token);

            var k = Console.ReadKey();
            if (k.Key == ConsoleKey.A) {
                cts.Cancel();
            }

        }

        static void Ex03() {
            //This example is equivalent to Ex15 of the Thread examples,
            //but it's faster because it does not spawn 200 new Threads
            //and it uses the ThreadPool instead.

            ExampleLibrary.BankScenario.BankAccount ba1 = new ExampleLibrary.BankScenario.BankAccount() { Id = 1 };
            ExampleLibrary.BankScenario.BankAccount ba2 = new ExampleLibrary.BankScenario.BankAccount() { Id = 2 };

            ba1.Deposit(100);

            ExampleLibrary.BankScenario.EventWaitHandles.Bank bank = new ExampleLibrary.BankScenario.EventWaitHandles.Bank();
            for (int i = 0; i < 100; i++) { //200 concurrent Threads running transfer
                ThreadPool.QueueUserWorkItem(_ => {
                    bank.Transfer(ba1, ba2, 50);
                    Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
                });
                ThreadPool.QueueUserWorkItem(_ => {
                    bank.Transfer(ba2, ba1, 50);
                    Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
                });
            }

        }

        static void Ex02() {
            //this will run as a background thread,
            //meaning that the main thread will kill this
            //if it terminates earlier

            void goBackground(object _){
                for (int i = 0; i < 10000; i++) {
                    Console.WriteLine($"Background Thread {Thread.CurrentThread.ManagedThreadId} running (if you press enter, the application exits before this ends)");
                }
            };

            ThreadPool.QueueUserWorkItem(goBackground, null);
        }

        static void Ex01() {
            //we can run a method on another thread
            //by using the threadpool.
            //the difference is that a new thread will be
            //created only if necessary.
            //for example if we only have 8 CPU
            //it would be useless to have more than 8 threads
            //at the same time, because we would have to 
            //switch between threads and that would cost
            //memory and time.
            
            void doYourThing(object o){
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} got {o}");
            };

            for (int i = 0; i < 26; i++) {
                ThreadPool.QueueUserWorkItem(doYourThing, (char)(i+65));
                //instead of
                //new Thread(doYourThing).Start('A'); etc...
            }
            
        }
    }
}
