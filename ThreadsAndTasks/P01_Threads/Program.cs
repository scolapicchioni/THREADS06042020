using System;
using System.Linq;
using System.Threading;
using ExampleLibrary;

namespace P01_Threads
{
    class Program
    {
        static void Main(string[] args)
        {
            Ex01();
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();

        }

        static void Ex24() {
            //in this scenario we want to store some data
            //that has to be unique on each thread.
            //we can use ThreadLocal

            ThreadLocal<int> ex24Data = new ThreadLocal<int>(() => 5); //the initial value is 5

            void a(){
                int tid = Thread.CurrentThread.ManagedThreadId;
                string tabs = new string(Enumerable.Repeat('\t', tid).ToArray());
                Console.WriteLine($"{tabs}a running on ThreadId: {tid} found {ex24Data.Value}");
                ex24Data.Value = ex24Data.Value * 10;
                Console.WriteLine($"{tabs}a running on ThreadId: {tid} changed data to {ex24Data.Value}");
            };

            void b(){
                int tid = Thread.CurrentThread.ManagedThreadId;
                string tabs = new string(Enumerable.Repeat('\t', tid).ToArray());
                Console.WriteLine($"{tabs}b running on ThreadId: {tid} found {ex24Data.Value}");
            };

            void run(object newValue){
                int tid = Thread.CurrentThread.ManagedThreadId;
                string tabs = new string(Enumerable.Repeat('\t', tid).ToArray());
                Console.WriteLine($"{tabs}ThreadId: {tid} found {ex24Data.Value}");
                ex24Data.Value = (int)newValue;
                a();
                b();
            };

            new Thread(run).Start(1);
            new Thread(run).Start(2);
            new Thread(run).Start(3);
        }

        //this initialization (5) takes place ONLY on the thread
        //that loads this class
        [ThreadStatic]
        static int ex23Data = 5; 
        static void Ex23() {
            //in this scenario we want to store some data
            //that has to be unique on each thread.
            //we can use the [ThreadStatic] attribute

            void a(){
                Console.WriteLine($"a running on ThreadId: {Thread.CurrentThread.ManagedThreadId} found {ex23Data}");
                ex23Data = ex23Data*10;
                Console.WriteLine($"a running on ThreadId: {Thread.CurrentThread.ManagedThreadId} changed data to {ex23Data}");
            };

            void b(){
                Console.WriteLine($"b running on ThreadId: {Thread.CurrentThread.ManagedThreadId} found {ex23Data}");
            };


            void run(object newValue){
                Console.WriteLine($"ThreadId: {Thread.CurrentThread.ManagedThreadId} found {ex23Data}");
                ex23Data = (int)newValue;
                a();
                b();
            };

            new Thread(run).Start(1);
            new Thread(run).Start(2);
            new Thread(run).Start(3);
        }

        static void Ex22() {
            //it's also possible to cancel a Thread,
            //but the cancellation is cooperative.
            //This means that it's the Thread 
            //that has to decide if and when
            //to actually quit. This gives the possibility
            //to undo or get to a state when it's safe
            //to exit, instead of abruptly quit
            //and risk to leave data in a dirty state.

            CancellationTokenSource cts = new CancellationTokenSource();

            Thread t = new Thread(o => {
                CancellationToken ct = (CancellationToken)o;
                Console.Clear();
                Console.WriteLine("Type A to abort");
                try {
                    int i = 0;
                    while (true) {
                        Console.CursorTop = 3;
                        Console.CursorLeft = 3;
                        Console.Write(i++);
                        //this is a good moment to actually stop,
                        //so we check if the cancellation has been
                        //requested and if so we throw an exeption
                        if (ct.IsCancellationRequested) { 
                            //undo
                        
                        }
                        //ct.ThrowIfCancellationRequested();
                    }

                } catch {
                    //here we decide what to do 
                    Console.WriteLine("kthxbye");
                    //undo stuff or just leave...
                }

            });
            t.Start(cts.Token);

            var k = Console.ReadKey();
            if (k.Key == ConsoleKey.A) {
                //we request a cancellation.
                //if and when the thread decides to stop,
                //it will. 
                cts.Cancel();
                //t.Start()
            }
        }

        static void Ex21() {
            //let's take a look at another scenario,
            //called Producer / Consumer.

            //Here we have one or more threads busy
            //with pushing data into a buffer (producers),
            //while one or more concurrent threads
            //are busy pulling the data out of the buffer (consumers).

            //we cannot work with a normal data structure
            //like a queue because it's not thread safe

            //what we can do is to wrap the unsafe queue
            //in a class that protects the code that
            //actually inserts and deletes the data from / to the queue.

            ExampleLibrary.ProducerConsumerScenario.Buffer<int> buffer = new ExampleLibrary.ProducerConsumerScenario.Buffer<int>();

            //this is our producer thread
            Thread producerThread = new Thread(() => {
                for (int i = 0; i < 100; i++) {
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} pushing {i}");
                    buffer.Push(i);
                }
                buffer.SendComplete();

            });

            //we create 3 consumers
            for (int i = 0; i < 3; i++) {
                new Thread(() => {
                    while (!buffer.IsCompleted) {
                        //we need to lock on the buffer simply because
                        //the console.writeline has to be executed 
                        //atomically with the pull.
                        //if we don't do that, the producer may run
                        //concurrently and we could see weird stuff on the screen.
                        Monitor.TryEnter(buffer);
                        if (Monitor.IsEntered(buffer)) {
                            int number = buffer.Pull();
                            Console.WriteLine($"\t{Thread.CurrentThread.ManagedThreadId} pulling {number}");
                        }
                    }
                }).Start();
            }

            producerThread.Start();
        }

        private static void Ex20() {
            //this bus starts empty and has 3 seats  
            SemaphoreSlim semaphore = new SemaphoreSlim(3, 3);

            void doStuff(){
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} trying to get on the bus. Current count: {semaphore.CurrentCount}");
                semaphore.Wait();
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} got on the bus. Current count: {semaphore.CurrentCount}");
                Thread.Sleep(1000);
                semaphore.Release();
                
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} got down the bus. Current count: {semaphore.CurrentCount}");
            };

            for (int i = 0; i < 10; i++) {
                new Thread(doStuff).Start();
            }
        }

        private static void Ex19() {
            //Another signaling class is the Barrier,
            //similar to the CountdownEvent.
            //When the CountdownEvent reaches 0, it's done.
            //When the Barrier reaches 0, instead, 
            //it restarts its counter.
            
            //Also, it allows us to run an action
            //everytime the counter reaches 0.
            void postPhase(Barrier b) {
                Console.WriteLine("!");
            };

            //the counter starts from 3
            Barrier barrier = new Barrier(3, postPhase);

            void doStuff(object letter){
                for (int i = 0; i < 5; i++) {
                    Console.Write($"{letter}{i} ");
                    //we decrease the counter 
                    //and wait until it reaches 0
                    barrier.SignalAndWait();
                }
            };

            for (int i = 0; i < 3; i++) {
                new Thread(doStuff).Start((char)(i + 65)); //we pass A, B and C
            }
        }

        static void SemaphoreExample() {
            Semaphore semaphore = new Semaphore(3, 3);

            for (int i = 0; i < 100; i++) {
                new Thread(() => {
                    semaphore.WaitOne();
                    Console.WriteLine("doing heavy stuff.....");
                    semaphore.Release();
                }).Start();
            }
            
        
        }

        static void CountDownEventSample() {
            CountdownEvent countdown = new CountdownEvent(5);
            new Thread(() => {
                //
                countdown.Signal();
            }).Start();

            new Thread(() => {
                //
                countdown.Signal();
            }).Start();

            new Thread(() => {
                //
                countdown.Signal();
            }).Start();

            new Thread(() => {
                //
                countdown.Signal();
            }).Start();

            new Thread(() => {
                //
                countdown.Signal();
            }).Start();

            countdown.Wait();
            Console.WriteLine("I'm here!");
        
        }


        static void AutoResetEventExample2() {
            EventWaitHandle[] eventWaitHandles = new EventWaitHandle[5] {
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false)
            };


            AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            new Thread(() => {
                //
                eventWaitHandles[0].Set();
            }).Start();

            new Thread(() => {
                //
                eventWaitHandles[1].Set();
            }).Start();

            new Thread(() => {
                //
                eventWaitHandles[2].Set();
            }).Start();

            new Thread(() => {
                //
                eventWaitHandles[3].Set();
            }).Start();

            new Thread(() => {
                //
                eventWaitHandles[4].Set();
            }).Start();

            WaitHandle.WaitAll(eventWaitHandles);


            Console.WriteLine("I'm here now!!");
        }

        static void AutoResetEventExample() {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            new Thread(()=> {
                //
                autoResetEvent.Set();
            }).Start();

            autoResetEvent.WaitOne();

            Console.WriteLine("I'm here now!!");
        }

        static void Ex18() {
            //In this scenario we want to wait for all Thread to finish
            //before we continue.
            //Another thing we could do is to use a CountdownEvent.

            CountdownEvent doneEvent = new CountdownEvent(200);

            ExampleLibrary.BankScenario.BankAccount ba1 = new ExampleLibrary.BankScenario.BankAccount() { Id = 1 };
            ExampleLibrary.BankScenario.BankAccount ba2 = new ExampleLibrary.BankScenario.BankAccount() { Id = 2 };

            ba1.Deposit(100);

            ExampleLibrary.BankScenario.EventWaitHandles.Bank bank = new ExampleLibrary.BankScenario.EventWaitHandles.Bank();
            for (int i = 0; i < 100; i++) { //200 concurrent Threads running transfer
                new Thread(() => {
                    bank.Transfer(ba1, ba2, 50);
                    Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
                    doneEvent.Signal();
                }).Start();
                new Thread(() => {
                    bank.Transfer(ba2, ba1, 50);
                    Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
                    doneEvent.Signal();
                }).Start();
            }
            doneEvent.Wait(); //waiting for it to reach 0
            Console.WriteLine("DONE!!");
        }

        static void Ex17() {
            //In this scenario we want to wait for all Thread to finish
            //before we continue.
            //Another thing we could do is to create just one event
            //and set it when a counter reaches 200.
            //BUT we have to increase the same counter on 200 different threads,
            //meaning that we have to do it in a thread safe manner.

            AutoResetEvent doneEvent = new AutoResetEvent(false);

            ExampleLibrary.BankScenario.BankAccount ba1 = new ExampleLibrary.BankScenario.BankAccount() { Id = 1 };
            ExampleLibrary.BankScenario.BankAccount ba2 = new ExampleLibrary.BankScenario.BankAccount() { Id = 2 };

            ba1.Deposit(100);

            ExampleLibrary.BankScenario.EventWaitHandles.Bank bank = new ExampleLibrary.BankScenario.EventWaitHandles.Bank();
            int counter = 0;
            for (int i = 0; i < 100; i++) { //200 concurrent Threads running transfer
                new Thread(() => {
                    bank.Transfer(ba1, ba2, 50);
                    Interlocked.Increment(ref counter);
                    Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
                    if (counter == 200) doneEvent.Set();

                }).Start();
                new Thread(() => {
                    bank.Transfer(ba2, ba1, 50);
                    Interlocked.Increment(ref counter);
                    Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
                    if (counter == 200) doneEvent.Set();
                }).Start();
            }
            doneEvent.WaitOne();
            Console.WriteLine("DONE!!");
        }

        static void Ex16() {
            //In this scenario we want to wait for all Thread to finish
            //before we continue.
            //We could create 200 AutoResetEvents, put them in an Array,
            //signal each one of them from a Thread
            //and use EventWaitHandle.WaitAll,
            //but they would be too many.

            AutoResetEvent[] events = Enumerable.Repeat(new AutoResetEvent(false), 200).ToArray();

            ExampleLibrary.BankScenario.BankAccount ba1 = new ExampleLibrary.BankScenario.BankAccount() { Id = 1 };
            ExampleLibrary.BankScenario.BankAccount ba2 = new ExampleLibrary.BankScenario.BankAccount() { Id = 2 };

            ba1.Deposit(100);

            ExampleLibrary.BankScenario.EventWaitHandles.Bank bank = new ExampleLibrary.BankScenario.EventWaitHandles.Bank();
            for (int i = 0; i < 100; i++) { //200 concurrent Threads running transfer
                new Thread(() => {
                    bank.Transfer(ba1, ba2, 50);
                    events[i].Set();
                    Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
                }).Start();
                new Thread(() => {
                    bank.Transfer(ba2, ba1, 50);
                    events[i + 100].Set();
                    Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
                }).Start();
            }
            //boom!
            WaitHandle.WaitAll(events);
            Console.WriteLine("DONE!!");
        }

        static void Ex15() {
            //this Bank has an Event it's waiting for
            //in order to proceed with the Transfer.

            ExampleLibrary.BankScenario.BankAccount ba1 = new ExampleLibrary.BankScenario.BankAccount() { Id = 1 };
            ExampleLibrary.BankScenario.BankAccount ba2 = new ExampleLibrary.BankScenario.BankAccount() { Id = 2 };

            ba1.Deposit(100);

            ExampleLibrary.BankScenario.EventWaitHandles.Bank bank = new ExampleLibrary.BankScenario.EventWaitHandles.Bank();
            for (int i = 0; i < 100; i++) { //200 concurrent Threads running transfer
                new Thread(() => {
                    bank.Transfer(ba1, ba2, 50);
                    Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
                }).Start();
                new Thread(() => {
                    bank.Transfer(ba2, ba1, 50);
                    Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
                }).Start();
            }

        }

        static void Ex14() {
            //this Bank tries to lock the bankAccounts it wants to use
            //for 10 milliseconds, after which it stops trying.
            //that's why this transfer could fail.

            //if multiple threads try to transfer from the same
            //accounts, the transfer could fail,
            //so we can decide if we want to keep trying,
            //or stop, or who knows what

            ExampleLibrary.BankScenario.BankAccount ba1 = new ExampleLibrary.BankScenario.BankAccount() { Id = 1 };
            ExampleLibrary.BankScenario.BankAccount ba2 = new ExampleLibrary.BankScenario.BankAccount() { Id = 2 };

            ba1.Deposit(100);

            ExampleLibrary.BankScenario.Timeout.Bank bank = new ExampleLibrary.BankScenario.Timeout.Bank();
            for (int i = 0; i < 100; i++) { //200 concurrent Threads running transfer
                new Thread(() => {
                    while (!bank.Transfer(ba1, ba2, 50)) { //it could timeout
                        Console.Write(".");
                    } //keep trying until we get it
                    Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
                }).Start();
                new Thread(() => {
                    while (!bank.Transfer(ba2, ba1, 50)) { //it could timeout
                        Console.Write(".");
                    } //keep trying until we get it
                    Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
                }).Start();
            }
            
        }

        static void Ex13() {
            //this Bank locks the bankAccounts it wants to use
            //when Transfering an amount from a source BankAccount
            //to a target BankAccount.

            //if multiple threads try to transfer from the same
            //accounts, deadlock problems could arise....

            ExampleLibrary.BankScenario.BankAccount ba1 = new ExampleLibrary.BankScenario.BankAccount() { Id = 1 };
            ExampleLibrary.BankScenario.BankAccount ba2 = new ExampleLibrary.BankScenario.BankAccount() { Id = 2 };

            ba1.Deposit(100);

            ExampleLibrary.BankScenario.Deadlocked.Bank bank = new ExampleLibrary.BankScenario.Deadlocked.Bank();
            for (int i = 0; i < 100; i++) { //200 concurrent Threads running transfer
                new Thread(() => {
                    bank.Transfer(ba1, ba2, 50);
                    Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
                }).Start();
                new Thread(() => {
                    bank.Transfer(ba2, ba1, 50);
                    Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
                }).Start();
            }
            //we could never get here.... 
        }

        static void Ex12() {
            //same Bank that 
            //can Transfer an amount from a source BankAccount
            //to a target BankAccount.

            //if multiple threads try to transfer from the same
            //accounts, concurrency problems could arise....

            ExampleLibrary.BankScenario.BankAccount ba1 = new ExampleLibrary.BankScenario.BankAccount() { Id = 1 };
            ExampleLibrary.BankScenario.BankAccount ba2 = new ExampleLibrary.BankScenario.BankAccount() { Id = 2 };

            ba1.Deposit(100);

            ExampleLibrary.BankScenario.NotSynchronized.Bank bank = new ExampleLibrary.BankScenario.NotSynchronized.Bank();
            for (int i = 0; i < 100; i++) { //200 concurrent Threads running transfer
                new Thread(()=> {
                    bank.Transfer(ba1, ba2, 50);
                    Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
                }).Start();
                new Thread(()=> {
                    bank.Transfer(ba2, ba1, 50);
                    Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
                }).Start();
            }
            //we could see weird numbers popping up.... 
        }

        static void Ex11() {
            //in our scenario we have a Bank that 
            //can Transfer an amount from a source BankAccount
            //to a target BankAccount.

            //if we do not use multithreading, 
            //no problems will arise

            ExampleLibrary.BankScenario.BankAccount ba1 = new ExampleLibrary.BankScenario.BankAccount() { Id = 1 };
            ExampleLibrary.BankScenario.BankAccount ba2 = new ExampleLibrary.BankScenario.BankAccount() { Id = 2 };

            ba1.Deposit(100);

            ExampleLibrary.BankScenario.NotSynchronized.Bank bank = new ExampleLibrary.BankScenario.NotSynchronized.Bank();
            for (int i = 0; i < 100; i++) { //200 transfer
                bank.Transfer(ba1, ba2, 50);
                bank.Transfer(ba2, ba1, 50);
            }
            Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}"); //ba1.Saldo == 100, ba2.Saldo == 0

        }

        static void InterlockedExample() {
            int counter = 0;

            for (int i = 0; i < 10; i++) {
                new Thread(() => Interlocked.Increment(ref counter)).Start();
            }
                
        }

        static void Ex10c() {
            //with the TryEnter we continue after a while 
            //if we didn't get a lock on time
            bool done = false;
            object lockOnDone = new object();

            void doYourThing() {

                try {
                    bool lockTaken = Monitor.TryEnter(lockOnDone);
                    //Am I here because I got the lock?
                    //Maybe, but maybe not...
                    if (lockTaken) { //let me check
                        if (!done) {
                            Console.WriteLine("done");
                            done = true;
                        }
                    } else { 
                        //so we didn't get the lock.. now what?
                        //up to you to decide....
                        //for example, you could return a boolean
                        //whether indicate that the operation was 
                        //completed successfully, so that the 
                        //caller can eventually try again later.
                        //or you could try yourself once again...
                        //maybe try 3 times and then return false?
                        //you can decide your strategy
                    }
                } finally {
                    Monitor.Exit(lockOnDone);
                }

            };

            new Thread(doYourThing).Start();
            new Thread(doYourThing).Start();

        }

        static void Ex10b() {
            //lock keyword is equivalent to the following code 
            //(more or less)
            bool done = false;
            object lockOnDone = new object();

            void doYourThing() {

                try {
                    Monitor.Enter(lockOnDone);
                    if (!done) {
                        Console.WriteLine("done");
                        done = true;
                    }
                } finally {
                    Monitor.Exit(lockOnDone);
                }

            };

            new Thread(doYourThing).Start();
            new Thread(doYourThing).Start();

        }

        static void Ex10a() {
            //with locks we can avoid some problems....
            //(more on this later...)
            bool done = false;
            object lockOnDone = new object();

            void doYourThing() {
                lock (lockOnDone)
                {
                    if (!done) {
                        Console.WriteLine("done");
                        done = true;
                    }
                }
            };

            new Thread(doYourThing).Start();
            new Thread(doYourThing).Start();

        }

        static void Ex09() {
            //another example of concurrency problems....
            bool done = false;

            void doYourThing(){
                if (!done) {
                    Console.WriteLine("done");
                    done = true;
                }
            };

            //calling the functions in the same thread causes no problem
            //doYourThing();
            //doYourThing();

            //but if we call them in different threads we MIGHT get unexpected results
            new Thread(doYourThing).Start();
            new Thread(doYourThing).Start();

        }

        static void Ex08() {
            //the problems start when multiple threads
            //share the same resources

            int sharedVariable = 0;

            void changeSharedVariable(){
                for (int i = 0; i < 100; i++) {
                    Thread.Sleep(10);
                    sharedVariable = sharedVariable + 1;
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} - sharedVariable == {sharedVariable}");
                }
            };

            //should get to 1000 but...
            for (int i = 0; i < 10; i++) {
                new Thread(changeSharedVariable).Start();
            }
        }

        static void Ex07() {
            //if the threads use their own local variables,
            //no problem will arise. We will always get from 0 to 4 both times.

            void print() {
                for (int i = 0; i < 5; i++) {
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} - i == {i}");
                }
            };

            new Thread(print).Start();
            print();
        }

        static void Ex06() {
            int calc(string s) {
                return s.Length;
            }

            //if you want to get the result of method
            //run in another thread, you can't do that
            //with a Thread (you can only pass methods 
            //that return void!)
            //But you could use a delegate and get the result in a callback:
            Func<string, int> calculate = calc;

            void callback (IAsyncResult asyncresult) {
                int result = calculate.EndInvoke(asyncresult);
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} has the result: {result}");
            }

            calculate.BeginInvoke("supercalifragilisticexpialidocious", callback, null);

        }

        static void Ex05() {
            //you can wait for a thread to be finished
            //by joining it

            int sharedVariable = 0;
            void go(){
                for (int i = 0; i < 1000; i++) {
                    Thread.Sleep(1);
                    sharedVariable++;
                }
            };

            Thread t = new Thread(go);
            t.Start();
            //if we don't wait, who knows what we're going to print...
            t.Join();
            Console.WriteLine(sharedVariable);
        }

        static void Ex04() {
            //forget catching an exception outside a thread....
            void go() {
                throw new Exception("AAAAAAAAAAAAAARGH!");
            };

            try {
                new Thread(()=> {
                    try {
                        go();
                    } catch (Exception ex) {
                        Console.WriteLine(ex);
                    }
                }).Start();
            } catch (Exception e) {
                Console.WriteLine($"caught {e}"); //or not.....
            }
        }

        static void Ex03() {
            /*
             * By default, threads you create explicitly 
             * are foreground threads. 
             * Foreground threads keep the application alive 
             * for as long as any one of them is running, 
             * whereas background threads do not. 
             * Once all foreground threads finish, 
             * the application ends, and any background threads 
             * still running abruptly terminate.
             */

            void go() {
                for (int i = 0; i < 10000; i++) {
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} running  as a {(Thread.CurrentThread.IsBackground ? "background" : "foreground")} thread. (if you press enter, the application {(Thread.CurrentThread.IsBackground ? "exits" : "does not exit")} before this ends)");
                }
            };
            
            //Thread t = new Thread(go);

            //this will run as a background thread,
            //meaning that the main thread will kill this
            //if it terminates earlier

            Thread t = new Thread(go) { IsBackground = true };
            
            t.Start();
        }

        static void Ex02() {
            //we can pass an object to a method 
            //during the Start of the new thread
            void doYourThing(object o) {
                Console.WriteLine(o);
            };

            int method01(int a, int b) {
                Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                return a + b;
            }

            new Thread(doYourThing).Start("a");
            new Thread(doYourThing).Start("b");
            new Thread(doYourThing).Start("c");

            int result = 0;
            Thread t = new Thread(() => {
                Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
                result = method01(3, 4);
            });
            t.Start();

            t.Join();
            Console.WriteLine(result);

        }

        static void Ex01() {
            void printX () {
                for (int i = 0; i < 10_000; i++) {
                    Console.Write("x");
                }
            };

            //deterministic: 
            //we will ALWAYS see 10000 x 
            //followed by 10000 o
            //printX();
            //for (int i = 0; i < 10_000; i++) {
            //    Console.Write("o");
            //}

            //non deterministic:
            //x and o will mix in a random way
            new Thread(printX).Start();
            for (int i = 0; i < 10_000; i++) {
                Console.Write("O");
            }
            /*
             * xxxxxxxxxxxxxxxxxxxxxxxxxxOOOOOOOOOOOOOOOOOOOOOOxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxOOOOOOOOOOOOOOOOOOOOOO
             * OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
             * OOOOOOOOOOOOxxxxxxxxxxxxxxxxxxxxxxxxOOOOOOOOOOOOOOOOOOOOOOOOOxxxxxxxxxxxxxxxxxxOOOOOxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxOOOOOOxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxOOOOOOOOOOOOxxxxxOOOOOOOOOOOOOOOOOOxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxOOOxxxxxxxxxOOOOOOOOxxxxOOO
             * OOOOOOOOOOOOOOOOOOOOOOOOOOOOOxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
             * OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOxxxxxxxxxxxxxxxxxxxxxxx
             * xxxOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOxxxxOOOOOOOOOOOOOOOOOOOO
             * OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
             * OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
             * OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxOOxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
             * OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
             * OOxxxxOOOOOOOOOOOOOOxxxxxxxxxxxxxxxxxxxxxOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
             * OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
             * OOOOOOxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
             * OOOOOOOOOOOOOOOOOOOOOOOOxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
             * OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
             * OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOxxxxxxxxxxxOOOOxxxxxxxxxxxxOOOOOOOOOOOO
             * OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxOOOOOOOOOOOOOOOOOOOxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
             * OOOOOOOOOOOOOOOOOOOOOOOOOxxxxxxxxxxOOOOOOOOOOOOOOOOOOOOOOxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOxxxOOOOxxxxOOOOOOOOOOOOOOOOOOOOOxxOOOOOxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
             * OOOOOOOOOOOOOOOOOOOOOOOOOOOOxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxOOOOOOOOOOOOOOOOOOOOOOxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxOOOOOOOOOOOOOOOOOOOOOOOOOOOxxxxx
             * xxxxxxxOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
             * OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxOOOOOOOOOOOOOOOOOOOO
             * OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxOO
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO
             * OOOOOOOOOOOOxxxxxxxxxxOOOOOOOxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             * xxxxxxxxOOOOOOOOOOOOOOOOOOxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             */
        }

        static void Ex00() {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId); //1
            Console.WriteLine(Thread.CurrentThread.IsBackground); //false
            Console.ReadLine();
        }

    }
}
