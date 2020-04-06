using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace P09_TaskSynchronization {
    class Program {
        static void Main(string[] args) {
            Ex15();
            Console.WriteLine("Press Enter to quit...");
            Console.ReadLine();
        }

        //static void Ex24() {
        //    //in this scenario we want to store some data
        //    //that has to be unique on each thread.
        //    //we cannot use ThreadLocal

        //    ThreadLocal<int> ex24Data = new ThreadLocal<int>(() => 5); //the initial value is 5

        //    void a() {
        //        int tid = Thread.CurrentThread.ManagedThreadId;
        //        string tabs = new string(Enumerable.Repeat('\t', tid).ToArray());
        //        Console.WriteLine($"{tabs}a running on ThreadId: {tid} found {ex24Data.Value}");
        //        ex24Data.Value = ex24Data.Value * 10;
        //        Console.WriteLine($"{tabs}a running on ThreadId: {tid} changed data to {ex24Data.Value}");
        //    };

        //    void b() {
        //        int tid = Thread.CurrentThread.ManagedThreadId;
        //        string tabs = new string(Enumerable.Repeat('\t', tid).ToArray());
        //        Console.WriteLine($"{tabs}b running on ThreadId: {tid} found {ex24Data.Value}");
        //    };

        //    void run(object newValue) {
        //        int tid = Thread.CurrentThread.ManagedThreadId;
        //        string tabs = new string(Enumerable.Repeat('\t', tid).ToArray());
        //        Console.WriteLine($"{tabs}ThreadId: {tid} found {ex24Data.Value}");
        //        ex24Data.Value = (int)newValue;
        //        a();
        //        b();
        //    };

        //    Task.Run(() => run(1));
        //    Task.Run(() => run(2));
        //    Task.Run(() => run(3));
        //}

        ////this initialization (5) takes place ONLY on the thread
        ////that loads this class
        //[ThreadStatic]
        //static int ex23Data = 5;
        //static void Ex23() {
        //    //in this scenario we want to store some data
        //    //that has to be unique on each thread.
        //    //we can use the [ThreadStatic] attribute

        //    void a() {
        //        Console.WriteLine($"a running on ThreadId: {Thread.CurrentThread.ManagedThreadId} found {ex23Data}");
        //        ex23Data = ex23Data * 10;
        //        Console.WriteLine($"a running on ThreadId: {Thread.CurrentThread.ManagedThreadId} changed data to {ex23Data}");
        //    };

        //    void b() {
        //        Console.WriteLine($"b running on ThreadId: {Thread.CurrentThread.ManagedThreadId} found {ex23Data}");
        //    };


        //    void run(object newValue) {
        //        Console.WriteLine($"ThreadId: {Thread.CurrentThread.ManagedThreadId} found {ex23Data}");
        //        ex23Data = (int)newValue;
        //        a();
        //        b();
        //    };

        //    new Thread(run).Start(1);
        //    new Thread(run).Start(2);
        //    new Thread(run).Start(3);
        //}

        //static void Ex22() {
        //    //it's also possible to cancel a Thread,
        //    //but the cancellation is cooperative.
        //    //This means that it's the Thread 
        //    //that has to decide if and when
        //    //to actually quit. This gives the possibility
        //    //to undo or get to a state when it's safe
        //    //to exit, instead of abruptly quit
        //    //and risk to leave data in a dirty state.
        //    //If we just exit, the caller won't know that it was canceled
        //    //If we want to "notify" the cancellation, we may want to
        //    //throw an exception that can be handled by the thread that waits.

        //    void work(CancellationToken token) {
        //        Console.Clear();
        //        Console.WriteLine("Type A to abort");
        //        int i = 0;
        //        while (true) {
        //            Console.CursorTop = 3;
        //            Console.CursorLeft = 3;
        //            Console.Write(i++);
        //            //this is a good moment to actually stop,
        //            //so we check if the cancellation has been
        //            //requested and if so we throw an exeption
        //            //in order to notify that we could not complete
        //            //our task and why
        //            token.ThrowIfCancellationRequested();

        //            //a different strategy would be to 
        //            //silently exit without complaining
        //            //but then the waiter would not know why we're done
        //            //if (token.IsCancellationRequested)
        //            //    break;
        //        }
        //    }

        //    CancellationTokenSource cts = new CancellationTokenSource();
        //    CancellationToken ct = cts.Token;
        //    try {
        //        Task.Run(() => work(ct), cts.Token).Wait();
        //    } catch(Exception ex) {
        //        Console.WriteLine($"{ex.GetType().Name} {ex.Message}");
        //        //here we decide what to do 
        //        Console.WriteLine("kthxbye");
        //        //undo stuff or just leave...
        //    }


        //    Task.Run(() => {
        //        var k = Console.ReadKey();
        //        if (k.Key == ConsoleKey.A) {
        //            //we request a cancellation.
        //            //if and when the thread decides to stop,
        //            //it will. 
        //            cts.Cancel();
        //        }
        //    });

        //}

        //static void Ex21() {
        //    //let's take a look at another scenario,
        //    //called Producer / Consumer.

        //    //Here we have one or more threads busy
        //    //with pushing data into a buffer (producers),
        //    //while one or more concurrent threads
        //    //are busy pulling the data out of the buffer (consumers).

        //    //we cannot work with a normal data structure
        //    //like a queue because it's not thread safe

        //    //what we can do is to wrap the unsafe queue
        //    //in a class that protects the code that
        //    //actually inserts and deletes the data from / to the queue.

        //    ExampleLibrary.ProducerConsumerScenario.Buffer<int> buffer = new ExampleLibrary.ProducerConsumerScenario.Buffer<int>();



        //    //we create 3 consumers
        //    for (int i = 0; i < 3; i++) {
        //        Task.Run(() => {
        //            while (!buffer.IsCompleted) {
        //                //we need to lock on the buffer simply because
        //                //the console.writeline has to be executed 
        //                //atomically with the pull.
        //                //if we don't do that, the producer may run
        //                //concurrently and we could see weird stuff on the screen.
        //                Monitor.TryEnter(buffer);
        //                if (Monitor.IsEntered(buffer)) {
        //                    int number = buffer.Pull();
        //                    Console.WriteLine($"\t{Thread.CurrentThread.ManagedThreadId} pulling {number}");
        //                }
        //            }
        //        });
        //    }

        //    //this is our producer Task
        //    Task.Run(() => {
        //        for (int i = 0; i < 100; i++) {
        //            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} pushing {i}");
        //            buffer.Push(i);
        //        }
        //        buffer.SendComplete();

        //    });


        //}

        //private static void Ex20() {
        //    //this bus starts empty and has 3 seats  
        //    SemaphoreSlim semaphore = new SemaphoreSlim(3, 3);

        //    void doStuff() {
        //        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} trying to get on the bus. Current count: {semaphore.CurrentCount}");
        //        semaphore.Wait();
        //        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} got on the bus. Current count: {semaphore.CurrentCount}");
        //        Task.Delay(1000).Wait();
        //        semaphore.Release();

        //        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} got down the bus. Current count: {semaphore.CurrentCount}");
        //    };

        //    for (int i = 0; i < 10; i++) {
        //        Task.Run(()=>doStuff());
        //    }
        //}

        //private static void Ex19() {
        //    //Another signaling class is the Barrier,
        //    //similar to the CountdownEvent.
        //    //When the CountdownEvent reaches 0, it's done.
        //    //When the Barrier reaches 0, instead, 
        //    //it restarts its counter.

        //    //Also, it allows us to run an action
        //    //everytime the counter reaches 0.
        //    void postPhase(Barrier b) {
        //        Console.WriteLine("!");
        //    };

        //    //the counter starts from 3
        //    Barrier barrier = new Barrier(3, postPhase);

        //    void doStuff(object letter) {
        //        for (int i = 0; i < 5; i++) {
        //            Console.Write($"{letter}{i} ");
        //            //we decrease the counter 
        //            //and wait until it reaches 0
        //            barrier.SignalAndWait();
        //        }
        //    };

        //    for (int i = 0; i < 3; i++) {
        //        Task.Factory.StartNew(doStuff, (char)(i + 65)); //we pass A, B and C
        //    }
        //}

        //static void Ex18() {
        //    //In this scenario we want to wait for all Thread to finish
        //    //before we continue.
        //    //Another thing we could do is to use a CountdownEvent.

        //    CountdownEvent doneEvent = new CountdownEvent(200);

        //    ExampleLibrary.BankScenario.BankAccount ba1 = new ExampleLibrary.BankScenario.BankAccount() { Id = 1 };
        //    ExampleLibrary.BankScenario.BankAccount ba2 = new ExampleLibrary.BankScenario.BankAccount() { Id = 2 };

        //    ba1.Deposit(100);

        //    ExampleLibrary.BankScenario.EventWaitHandles.Bank bank = new ExampleLibrary.BankScenario.EventWaitHandles.Bank();
        //    for (int i = 0; i < 100; i++) { //200 concurrent Threads running transfer
        //        Task.Run(() => {
        //            bank.Transfer(ba1, ba2, 50);
        //            Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
        //            doneEvent.Signal();
        //        });
        //        Task.Run(() => {
        //            bank.Transfer(ba2, ba1, 50);
        //            Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
        //            doneEvent.Signal();
        //        });
        //    }
        //    doneEvent.Wait(); //waiting for it to reach 0
        //    Console.WriteLine("DONE!!");
        //}

        //static void Ex17() {
        //    //In this scenario we want to wait for all Thread to finish
        //    //before we continue.
        //    //Another thing we could do is to create just one event
        //    //and set it when a counter reaches 200.
        //    //BUT we have to increase the same counter on 200 different threads,
        //    //meaning that we have to do it in a thread safe manner.

        //    AutoResetEvent doneEvent = new AutoResetEvent(false);

        //    ExampleLibrary.BankScenario.BankAccount ba1 = new ExampleLibrary.BankScenario.BankAccount() { Id = 1 };
        //    ExampleLibrary.BankScenario.BankAccount ba2 = new ExampleLibrary.BankScenario.BankAccount() { Id = 2 };

        //    ba1.Deposit(100);

        //    ExampleLibrary.BankScenario.EventWaitHandles.Bank bank = new ExampleLibrary.BankScenario.EventWaitHandles.Bank();
        //    int counter = 0;
        //    for (int i = 0; i < 100; i++) { //200 concurrent Threads running transfer
        //        Task.Run(() => {
        //            bank.Transfer(ba1, ba2, 50);
        //            Interlocked.Increment(ref counter);
        //            Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
        //            if (counter == 200) doneEvent.Set();

        //        });
        //        Task.Run(() => {
        //            bank.Transfer(ba2, ba1, 50);
        //            Interlocked.Increment(ref counter);
        //            Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
        //            if (counter == 200) doneEvent.Set();
        //        });
        //    }
        //    doneEvent.WaitOne();
        //    Console.WriteLine("DONE!!");
        //}

        //static void Ex16() {
        //    //In this scenario we want to wait for all Thread to finish
        //    //before we continue.
        //    //We could create 200 AutoResetEvents, put them in an Array,
        //    //signal each one of them from a Thread
        //    //and use EventWaitHandle.WaitAll,
        //    //but they would be too many.

        //    AutoResetEvent[] events = Enumerable.Repeat(new AutoResetEvent(false), 200).ToArray();

        //    ExampleLibrary.BankScenario.BankAccount ba1 = new ExampleLibrary.BankScenario.BankAccount() { Id = 1 };
        //    ExampleLibrary.BankScenario.BankAccount ba2 = new ExampleLibrary.BankScenario.BankAccount() { Id = 2 };

        //    ba1.Deposit(100);

        //    ExampleLibrary.BankScenario.EventWaitHandles.Bank bank = new ExampleLibrary.BankScenario.EventWaitHandles.Bank();
        //    for (int i = 0; i < 100; i++) { //200 concurrent Threads running transfer
        //        Task.Run(() => {
        //            bank.Transfer(ba1, ba2, 50);
        //            events[i].Set();
        //            Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
        //        });
        //        Task.Run(() => {
        //            bank.Transfer(ba2, ba1, 50);
        //            events[i + 100].Set();
        //            Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
        //        });
        //    }
        //    //boom!
        //    WaitHandle.WaitAll(events);
        //    Console.WriteLine("DONE!!");
        //}

        //static void Ex15() {
        //    //this Bank has an Event it's waiting for
        //    //in order to proceed with the Transfer.

        //    ExampleLibrary.BankScenario.BankAccount ba1 = new ExampleLibrary.BankScenario.BankAccount() { Id = 1 };
        //    ExampleLibrary.BankScenario.BankAccount ba2 = new ExampleLibrary.BankScenario.BankAccount() { Id = 2 };

        //    ba1.Deposit(100);

        //    ExampleLibrary.BankScenario.EventWaitHandles.Bank bank = new ExampleLibrary.BankScenario.EventWaitHandles.Bank();
        //    for (int i = 0; i < 100; i++) { //200 concurrent Threads running transfer
        //        Task.Run(() => {
        //            bank.Transfer(ba1, ba2, 50);
        //            Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
        //        });
        //        Task.Run(() => {
        //            bank.Transfer(ba2, ba1, 50);
        //            Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
        //        });
        //    }

        //}

        //static void Ex14() {
        //    //this Bank tries to lock the bankAccounts it wants to use
        //    //for 10 milliseconds, after which it stops trying.
        //    //that's why this transfer could fail.

        //    //if multiple threads try to transfer from the same
        //    //accounts, the transfer could fail,
        //    //so we can decide if we want to keep trying,
        //    //or stop, or who knows what

        //    //the problem here is that the Pulse
        //    //could be lost if it's executed BEFORE the Wait,
        //    //so we get a deadlock again!

        //    ExampleLibrary.BankScenario.BankAccount ba1 = new ExampleLibrary.BankScenario.BankAccount() { Id = 1 };
        //    ExampleLibrary.BankScenario.BankAccount ba2 = new ExampleLibrary.BankScenario.BankAccount() { Id = 2 };

        //    ba1.Deposit(100);

        //    ExampleLibrary.BankScenario.WaitPulse.Bank bank = new ExampleLibrary.BankScenario.WaitPulse.Bank();
        //    for (int i = 0; i < 100; i++) { //200 concurrent Threads running transfer
        //        Task.Run(() => {
        //            while (!bank.Transfer(ba1, ba2, 50)) {
        //                Console.Write(".");
        //            } //keep trying until we get it
        //            Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
        //        });
        //        Task.Run(() => {
        //            while (!bank.Transfer(ba2, ba1, 50)) {
        //                Console.Write(".");
        //            } //keep trying until we get it
        //            Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
        //        });
        //    }

        //}

        //static void Ex13() {
        //    //this Bank locks the bankAccounts it wants to use
        //    //when Transfering an amount from a source BankAccount
        //    //to a target BankAccount.

        //    //if multiple threads try to transfer from the same
        //    //accounts, deadlock problems could arise....

        //    ExampleLibrary.BankScenario.BankAccount ba1 = new ExampleLibrary.BankScenario.BankAccount() { Id = 1 };
        //    ExampleLibrary.BankScenario.BankAccount ba2 = new ExampleLibrary.BankScenario.BankAccount() { Id = 2 };

        //    ba1.Deposit(100);

        //    ExampleLibrary.BankScenario.Deadlocked.Bank bank = new ExampleLibrary.BankScenario.Deadlocked.Bank();
        //    for (int i = 0; i < 100; i++) { //200 concurrent Threads running transfer
        //        Task.Run(() => {
        //            bank.Transfer(ba1, ba2, 50);
        //            Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
        //        });
        //        Task.Run(() => {
        //            bank.Transfer(ba2, ba1, 50);
        //            Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
        //        });
        //    }
        //    //we could never get here.... 
        //}

        //static void Ex12() {
        //    //same Bank that 
        //    //can Transfer an amount from a source BankAccount
        //    //to a target BankAccount.

        //    //if multiple threads try to transfer from the same
        //    //accounts, concurrency problems could arise....

        //    ExampleLibrary.BankScenario.BankAccount ba1 = new ExampleLibrary.BankScenario.BankAccount() { Id = 1 };
        //    ExampleLibrary.BankScenario.BankAccount ba2 = new ExampleLibrary.BankScenario.BankAccount() { Id = 2 };

        //    ba1.Deposit(100);

        //    ExampleLibrary.BankScenario.NotSynchronized.Bank bank = new ExampleLibrary.BankScenario.NotSynchronized.Bank();
        //    for (int i = 0; i < 100; i++) { //200 concurrent Threads running transfer
        //        Task.Run(() => {
        //            bank.Transfer(ba1, ba2, 50);
        //            Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
        //        });
        //        Task.Run(() => {
        //            bank.Transfer(ba2, ba1, 50);
        //            Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}");
        //        });
        //    }
        //    //we could see weird numbers popping up.... 
        //}

        //static void Ex11() {
        //    //in our scenario we have a Bank that 
        //    //can Transfer an amount from a source BankAccount
        //    //to a target BankAccount.

        //    //if we do not use multithreading, 
        //    //no problems will arise

        //    ExampleLibrary.BankScenario.BankAccount ba1 = new ExampleLibrary.BankScenario.BankAccount() { Id = 1 };
        //    ExampleLibrary.BankScenario.BankAccount ba2 = new ExampleLibrary.BankScenario.BankAccount() { Id = 2 };

        //    ba1.Deposit(100);

        //    ExampleLibrary.BankScenario.NotSynchronized.Bank bank = new ExampleLibrary.BankScenario.NotSynchronized.Bank();
        //    for (int i = 0; i < 100; i++) { //200 transfer
        //        bank.Transfer(ba1, ba2, 50);
        //        bank.Transfer(ba2, ba1, 50);
        //    }
        //    Console.WriteLine($"{ba1.Id} == {ba1.Saldo}\t{ba2.Id} == {ba2.Saldo}"); //ba1.Saldo == 100, ba2.Saldo == 0

        //}

        static void Ex17() {
            //Event Wait Handles

            //AutoResetEvent

            /*
             * An AutoResetEvent is like a ticket turnstile: 
             * inserting a ticket lets exactly one person through. 
             * The “auto” in the class’s name refers to the fact 
             * that an open turnstile automatically closes 
             * or “resets” after someone steps through. 
             * A thread waits, or blocks, at the turnstile 
             * by calling WaitOne 
             * (wait at this “one” turnstile until it opens), 
             * and a ticket is inserted by calling the Set method. 
             * If a number of threads call WaitOne, 
             * a queue builds up behind the turnstile. 
             * 
             * A ticket can come from any thread; 
             * in other words, any (unblocked) thread 
             * with access to the AutoResetEvent object 
             * can call Set on it to release one blocked thread.
             * 
             */

            EventWaitHandle _waitHandle = new AutoResetEvent(false); //the door is closed

            void Waiter() {
                Console.WriteLine("Waiting...");
                _waitHandle.WaitOne();                // Wait for notification
                Console.WriteLine("Notified");        // Now the door is closed again
            }

            Task.Run(() => Waiter());
            Task.Delay(1000).Wait();              // Pause for a second...
            _waitHandle.Set();                    // Wake up the Waiter by opening the door.
            
        }

        static void Ex16() {
            //SemaphoreSlim

            /*
             * A semaphore is like a nightclub: 
             * it has a certain capacity, enforced by a bouncer. 
             * Once it’s full, no more people can enter, 
             * and a queue builds up outside. 
             * Then, for each person that leaves, 
             * one person enters from the head of the queue. 
             * A semaphore with a capacity of one 
             * is similar to a Mutex or lock, 
             * except that the semaphore has no “owner” — 
             * it’s thread-agnostic. 
             * Any thread can call Release on a Semaphore, 
             * whereas with Mutex and lock, 
             * only the thread that obtained the lock can release it.
             * Semaphores can be useful in limiting concurrency — 
             * preventing too many threads from executing 
             * a particular piece of code at once. 
             * In the following example, ten threads try 
             * to enter a nightclub that allows 
             * only three threads in at once:
            */

            SemaphoreSlim _sem = new SemaphoreSlim(3);    // Capacity of 3
            
            void enter(object id) {
                Console.WriteLine(id + " wants to enter");
                _sem.Wait();
                Console.WriteLine(id + " is in!");        // Only three threads
                Task.Delay(1000 * (int)id).Wait();               // can be here at
                Console.WriteLine(id + " is leaving");    // a time.
                _sem.Release();
            }

            for (int i = 1; i <= 10; i++)
                Task.Factory.StartNew(enter,i);

        }

        static void Ex15() {
            //Wait and Pulse

            object _locker = new object();

            bool go = false;

            void work() {
                Console.WriteLine($"WORK {Thread.CurrentThread.ManagedThreadId} wants in");
                lock (_locker) {
                    Console.WriteLine($"WORK {Thread.CurrentThread.ManagedThreadId} is in");
                    while (!go) { //blocking condition
                        Console.WriteLine($"WORK {Thread.CurrentThread.ManagedThreadId} it's a no go! releasing the lock");
                        Monitor.Wait(_locker); //we release the lock here
                        Console.WriteLine($"WORK {Thread.CurrentThread.ManagedThreadId} acquired the lock back");
                    }
                    Console.WriteLine($"WORK {Thread.CurrentThread.ManagedThreadId} is doing things");
                }
                Console.WriteLine($"WORK {Thread.CurrentThread.ManagedThreadId} is done, lock released");
            }
            
            void unblock() {
                Console.WriteLine($"\tUNBLOCK {Thread.CurrentThread.ManagedThreadId} wants in");
                lock (_locker) {
                    Console.WriteLine($"\tUNBLOCK {Thread.CurrentThread.ManagedThreadId} is in");
                    go = true; //we change the lock condition, so we pulse right after
                    Console.WriteLine($"\tUNBLOCK {Thread.CurrentThread.ManagedThreadId} set go to true, pulsing...");
                    Monitor.Pulse(_locker);  // or: Monitor.PulseAll (_locker);
                    Console.WriteLine($"\tUNBLOCK {Thread.CurrentThread.ManagedThreadId} pulsed");
                }
                Console.WriteLine($"\tUNBLOCK {Thread.CurrentThread.ManagedThreadId} released the lock");
            }

            Task.Run(() => work());
            Task.Run(() => unblock());

        }

        static void Ex14() {
            //deadlock

            object locker1 = new object();
            object locker2 = new object();

            Task.Run(() => {
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} wants locker1");
                lock (locker1) {
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} got locker1");
                    Task.Delay(1000).Wait();
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} wants locker2");
                    lock (locker2) { // Deadlock
                        Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} got locker2");
                    }
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} released locker2");
                }
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} released locker1");
            });

            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} wants locker2");
            lock (locker2) {
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} got locker2");
                Task.Delay(1000).Wait();
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} wants locker1");
                lock (locker1) {
                    Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} got locker1");
                } // Deadlock
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} released locker1");
            }
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} released locker2");
        }
        static void Ex13b() {
            //m1 and m2 run in two different threads so they wait for each other
            object locker = new object();
            void m1() {
                Console.WriteLine("m1 wants in");
                lock (locker) {
                    Console.WriteLine("m1 is in");
                    for (int i = 0; i < 100000; i++) {

                    }
                }
                Console.WriteLine("m1 is out");
            }

            void m2() {
                Console.WriteLine("m2 wants in");
                lock (locker) {
                    Console.WriteLine("m2 is in");
                    for (int i = 0; i < 100000; i++) {

                    }
                }
                Console.WriteLine("m2 is out");
            }

            Task.Run(() => m1());
            Task.Run(() => m2());

        }

        static void Ex13() {
            //nested locking
            //A thread can repeatedly lock the same object in a nested (reentrant) fashion:
            object locker = new object();
            void m1() {
                Console.WriteLine("m1 wants in");
                lock (locker) {
                    Console.WriteLine("m1 is in");
                    m2();
                }
                Console.WriteLine("m1 is out");
            }

            void m2() {
                Console.WriteLine("m2 wants in");
                lock (locker) {
                    Console.WriteLine("m2 is in");
                }
                Console.WriteLine("m2 is out");
            }

            m1();
        }

        static void Ex12() {
            //Monitor also has a TryEnter where we can specify a timeout

            object _locker = new object();

            void work() {
                bool lockTaken = false;
                try {
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} waiting for the lock, for a max of 5 seconds...");
                    Monitor.TryEnter(_locker, 5000, ref lockTaken);
                    if (lockTaken) {
                        Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} entered the lock");
                        for (int i = 0; i < 10000; i++) {
                            //doing hard work here...
                        };
                    } else {
                        Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} is tired of waiting..... bye!");
                    }
                } finally {
                    if (lockTaken) {
                        Monitor.Exit(_locker);
                        Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} exited the lock");
                    }
                }
            }

            void askUser() {
                lock (_locker) {
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} took the lock!");
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} waiting for the user press a key in order to Release the lock");
                    Console.ReadKey();
                    Console.WriteLine("kthxbye");
                }
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} released the lock");
            }

            Task[] tasks = {
                Task.Run(() => askUser()),
                Task.Run(() => work()) };
            Task.WaitAll(tasks);
        }

        static void Ex11() {
            //"lock" is a C# keyword
            //that is compiled more or less
            //like this:

            object _locker = new object();

            void work() {
                bool lockTaken = false;
                try {
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} waiting for the lock");
                    Monitor.Enter(_locker, ref lockTaken);
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} entered the lock");
                    for (int i = 0; i < 10000; i++) {
                        //doing hard work here...
                    }; 
                } finally {
                    if (lockTaken) {
                        Monitor.Exit(_locker);
                        Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} exited the lock");
                    }
                }
            }

            Task.Run(() => work());
            Task.Run(() => work());
            Task.Run(() => work());

        }

        static void Ex10() {
            //with locks we can avoid some problems....
            //(more on this later...)
            bool done = false;
            object lockOnDone = new object();

            void doYourThing() {
                lock (lockOnDone) {
                    if (!done) {
                        Console.WriteLine("done");
                        done = true;
                    }
                }
            };

            Task.Run(() => doYourThing());
            Task.Run(() => doYourThing());

        }

        static void Ex09() {
            //another example of concurrency problems....
            bool done = false;

            void doYourThing() {
                if (!done) {
                    Console.WriteLine("done");
                    done = true;
                }
            };

            //calling the functions in the same thread causes no problem
            //doYourThing();
            //doYourThing();

            //but if we call them in different threads we MIGHT get unexpected results
            Task.Run(()=>doYourThing());
            Task.Run(()=>doYourThing());

        }

        static void Ex08() {
            //the problems start when multiple Tasks
            //share the same resources

            int sharedVariable = 0;

            void changeSharedVariable() {
                for (int i = 0; i < 10; i++) {
                    var temp = sharedVariable;
                    Task.Delay(10).Wait();
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} - sharedVariable == {sharedVariable}");
                    sharedVariable = temp + 1;
                    Task.Delay(10).Wait();
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} - sharedVariable == {sharedVariable}");
                }
            };

            //should get to 1000 but...
            for (int i = 0; i < 100; i++) {
                Task.Run(()=>changeSharedVariable());
            }
        }

        static void Ex07() {
            //if the Tasks use their own local variables,
            //no problem will arise. We will always get from 0 to 4 both times.

            void print() {
                for (int i = 0; i < 5; i++) {
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} - i == {i}");
                }
            };

            Task.Run(()=>print());
            print();
        }

        static void Ex06() {
            int calc(string s) {
                Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} is calculating the length of {s}");
                return s.Length;
            }

            //if you want to get the result of method
            //run in another Task, you can do it by using Task<T>
            //which has a Result property.
            //BEWARE: reading the Result property BLOCKS the thread until the result is available
            
            Task<int> t = Task.Run(() => calc("supercalifragilisticexpialidocious"));
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} has the result: {t.Result}");
            
        }

        static void Ex05() {
            //you can wait for a task to be finished
            //by calling Wait (but you block the main thread this way)

            int sharedVariable = 0;
            void go() {
                for (int i = 0; i < 1000; i++) {
                    Task.Delay(1).Wait();
                    sharedVariable++;
                }
            };

            Task t = Task.Run(()=>go());
            
            //if we don't wait, who knows what we're going to print...
            t.Wait();
            Console.WriteLine(sharedVariable);
        }

        static void Ex04b() {
            //if you don't wait for it, the exception is lost....
            void go() {
                throw new Exception("AAAAAAAAAAAAAARGH!");
            };

            try {
                Task.Run((Action)go);
            } catch (Exception e) {
                Console.WriteLine($"caught {e}");
            }
        }

        static void Ex04() {
            //you can catch an exception outside a task, but only if you wait for it....
            void go() {
                throw new Exception("AAAAAAAAAAAAAARGH!");
            };

            try {
                Task.Run((Action)go).Wait();
            } catch (Exception e) {
                Console.WriteLine($"caught {e}"); 
            }
        }

        static void Ex03() {
            /*
             * By default, Tasks you create explicitly 
             * will spawn background threads. 
             * Once the application ends, any background thread 
             * still running abruptly terminates.
             */

            void go() {
                for (int i = 0; i < 10000; i++) {
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} running  as a {(Thread.CurrentThread.IsBackground ? "background" : "foreground")} thread. (if you press enter, the application {(Thread.CurrentThread.IsBackground ? "exits" : "does not exit")} before this ends)");
                }
            };

            //this will run as a background thread,
            //meaning that the main thread will kill this
            //if it terminates earlier

            Task.Run((Action)go);
            
        }

        static void Ex02b() {
            //we can pass an object to a method 
            //by using lambda expressions
            void doYourThing(object o) {
                Console.WriteLine(o);
            };

            Task.Run(() => doYourThing("a"));
            Task.Run(() => doYourThing("b"));
            Task.Run(() => doYourThing("c"));
        }

        static void Ex02() {
            //we can pass an object to a method 
            //during the StartNew of the TaskFactory
            void doYourThing(object o) {
                Console.WriteLine(o);
            };

            Task.Factory.StartNew(doYourThing, "a");
            Task.Factory.StartNew(doYourThing, "b");
            Task.Factory.StartNew(doYourThing, "c");
        }

        static void Ex01() {
            void printX() {
                for (int i = 0; i < 10000; i++) {
                    Console.Write("-");
                }
            };

            //deterministic: 
            //we will ALWAYS see 10000 x 
            //followed by 10000 -
            //printX();
            //for (int i = 0; i < 10000; i++) {
            //    Console.Write("o");
            //}

            //non deterministic:
            //x and o will mix in a random way
            Task.Run(()=>printX());
            for (int i = 0; i < 10000; i++) {
                Console.Write("O");
            }
        }

    }
}
