using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace P06_Tasks
{
    class Program
    {
        static void Main(string[] args)
        {
            //https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-based-asynchronous-programming

            Ex17();

            Console.WriteLine("Press Enter To Exit");
            Console.ReadLine();
        }

        static void Ex17() {
            Task task = new Task(() => 
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId}"));
            task.Start();


            TaskCompletionSource<int> taskCompletionSource = new TaskCompletionSource<int>();
            Task<int> t = taskCompletionSource.Task;
            for (int i = 0; i < 1_000_000; i++) {

            }
            taskCompletionSource.SetResult(10);
            
        }

        //static async void Ex15() {
        //    async IAsyncEnumerable<int> FetchIOTData() {
        //        for (int i = 1; i <= 10; i++) {
        //            await Task.Delay(1000);//Simulate waiting for data to come through. 
        //            yield return i;
        //        }
        //    }
        //  
        //    await foreach(var item in FetchIOTData()){
        //       Console.WriteLine(item)
        //    }
        //}

        static void Ex14() {
            /*
             * The System.Threading.Tasks.Task 
             * and System.Threading.Tasks.Task<TResult> classes 
             * support cancellation through the use of cancellation tokens 
             * in the .NET Framework. 
             * In the Task classes, cancellation involves cooperation 
             * between the user delegate, which represents 
             * a cancelable operation and the code that requested the cancellation. 
             * A successful cancellation involves the requesting code 
             * calling the CancellationTokenSource.Cancel method, 
             * and the user delegate terminating the operation in a timely manner. 
             * You can terminate the operation by using one of these options:
             * 
             * By simply returning from the delegate. 
             * In many scenarios this is sufficient; however, 
             * a task instance that is canceled in this way transitions 
             * to the TaskStatus.RanToCompletion state, 
             * not to the TaskStatus.Canceled state.
             * 
             * By throwing a OperationCanceledException 
             * and passing it the token on which cancellation was requested. 
             * The preferred way to do this is 
             * to use the ThrowIfCancellationRequested method. 
             * A task that is canceled in this way transitions 
             * to the Canceled state, which the calling code 
             * can use to verify that the task responded 
             * to its cancellation request.
             */


            var tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;

            Task task = Task.Factory.StartNew(() => {

                // Were we already canceled?
                ct.ThrowIfCancellationRequested();

                bool moreToDo = true;
                while (moreToDo) {
                    // Poll on this property if you have to do
                    // other cleanup before throwing.
                    if (ct.IsCancellationRequested) {
                        // Clean up here, then...
                        ct.ThrowIfCancellationRequested();
                    }

                }
            }, tokenSource.Token); // Pass same token to StartNew.

            tokenSource.Cancel();

            // Just continue on this thread, or Wait/WaitAll with try-catch:
            try {
                task.Wait();
            } catch (AggregateException e) {
                foreach (var v in e.InnerExceptions)
                    Console.WriteLine(e.Message + " " + v.Message);
            } finally {
                tokenSource.Dispose();
            }
        }

        static void Ex13() {
            //Unlike detached child tasks, attached 
            //child tasks are closely synchronized with the parent. 

            Task parent = Task.Factory.StartNew(() => {
                Console.WriteLine("Parent task executing.");
                Task child = Task.Factory.StartNew(() => {
                    Console.WriteLine("Attached child starting.");
                    Thread.SpinWait(5000000);
                    Console.WriteLine("Attached child completing.");
                }, TaskCreationOptions.AttachedToParent);
            });
            parent.Wait();
            Console.WriteLine("Parent has completed.");
        }

        static void Ex12() {
            /*
             * A child task (or nested task) is a 
             * System.Threading.Tasks.Task instance 
             * that is created in the user delegate 
             * of another task, which is known as the parent task. 
             * A child task can be either detached or attached. 
             * A detached child task is a task that 
             * executes independently of its parent. 
             * An attached child task is a nested task 
             * that is created with the TaskCreationOptions.AttachedToParent 
             * option whose parent does not explicitly 
             * or by default prohibit it from being attached.
             * 
             * Although a child task is created by a parent task, 
             * by default it is independent of the parent task.
             */

            Task parent = Task.Factory.StartNew(() => {
                Console.WriteLine("Outer task executing.");

                Task child = Task.Factory.StartNew(() => {
                    Console.WriteLine("Nested task starting.");
                    Thread.SpinWait(500000);
                    Console.WriteLine("Nested task completing.");
                });
            });

            parent.Wait();
            Console.WriteLine("Outer has completed.");
        }

        static void Ex11() {
            //Another way to deal with exceptions is to 
            //specify different continuations for exceptional 
            //versus nonexceptional outcomes. 
            //This is done with TaskContinuationOptions:

            Task task1 = Task.Factory.StartNew(() =>  throw null);

            Task error = task1.ContinueWith(ant => Console.Write(ant.Exception),
                                             TaskContinuationOptions.OnlyOnFaulted);

            Task ok = task1.ContinueWith(ant => Console.Write("Success!"),
                                          TaskContinuationOptions.NotOnFaulted);
        }

        static void Ex10() {

            //A continuation can find out if an exception was thrown 
            //by the antecedent via the antecedent task’s Exception property.
            //The following writes the details of a NullReferenceException to the console:

            Task task1 = Task.Factory.StartNew(() =>  throw null);
            Task task2 = task1.ContinueWith(ant => Console.Write(ant.Exception));
        }

        static void Ex09() {
            //You can also create a continuation that will run 
            //when any or all of a group of tasks has completed. 
            //To execute a continuation when all antecedent tasks have completed, 
            //you call the static Task.WhenAll method 

            List<Task<int>> tasks = new List<Task<int>>();
            for (int ctr = 1; ctr <= 10; ctr++) {
                int baseValue = ctr;
                tasks.Add(Task.Factory.StartNew((b) => {
                    int i = (int)b;
                    return i * i;
                }, baseValue));
            }
            //this does not block the current thread
            var continuation = Task.WhenAll(tasks);

            long sum = 0;
            //.Result may block the current thread if the result is not yet available
            for (int ctr = 0; ctr <= continuation.Result.Length - 1; ctr++) {
                Console.Write($"{continuation.Result[ctr]} {(ctr == continuation.Result.Length - 1 ? "=" : "+")} ");
                sum += continuation.Result[ctr];
            }
            Console.WriteLine(sum);

            //you could otherwise continue in a callback, where you receive a task whose result collates all the results in an array
            continuation.ContinueWith(tasksWithResults => {
                for (int ctr = 0; ctr <= tasksWithResults.Result.Length - 1; ctr++) {
                    Console.Write($"{tasksWithResults.Result[ctr]} {(ctr == tasksWithResults.Result.Length - 1 ? "=" : "+")} ");
                    sum += tasksWithResults.Result[ctr];
                }
                Console.WriteLine(sum);
            });
        }

        static void Ex08() {
            //Because Task.ContinueWith is an instance method, 
            //you can chain method calls together instead of instantiating 
            //a Task<TResult> object for each antecedent task. 
            //The following example is functionally identical 
            //to the previous example, except that it chains together 
            //calls to the Task.ContinueWith method. 
            //Note that the Task<TResult> object returned 
            //by the chain of method calls is the final continuation task.

            Task<int> t3 = Task.Run(() => {
                Console.WriteLine($"t1 - ThreadId: {Thread.CurrentThread.ManagedThreadId}");
                return 42;
            }).ContinueWith(t => {
                Console.WriteLine($"t2 - received {t.Result} on ThreadId: {Thread.CurrentThread.ManagedThreadId}");
                return t.Result + 1;
            }).ContinueWith(t => {
                Console.WriteLine($"t3 - received {t.Result} on ThreadId: {Thread.CurrentThread.ManagedThreadId}");
                return t.Result + 1;
            });

            Console.WriteLine($"Main - ThreadId: {Thread.CurrentThread.ManagedThreadId}");
        }

        static void Ex07() {
            //The Task.ContinueWith and Task<TResult>.ContinueWith methods 
            //let you specify a task to start when the antecedent task finishes. 
            //The delegate of the continuation task is passed a reference 
            //to the antecedent task so that it can examine the antecedent task's status 
            //and, by retrieving the value of the Task<TResult>.Result property, 
            //can use the output of the antecedent as input for the continuation.

            Task<int> t1 = Task.Run(() => {
                Console.WriteLine($"t1 - ThreadId: {Thread.CurrentThread.ManagedThreadId}");
                return 42;
            });

            Task<int> t2 = t1.ContinueWith(t => {
                Console.WriteLine($"t2 - received {t.Result} on ThreadId: {Thread.CurrentThread.ManagedThreadId}");
                return t.Result + 1;
            });

            Task<int> t3 = t2.ContinueWith(t => {
                Console.WriteLine($"t3 - received {t.Result} on ThreadId: {Thread.CurrentThread.ManagedThreadId}");
                return t.Result + 1;
            });

            Console.WriteLine($"Main - ThreadId: {Thread.CurrentThread.ManagedThreadId}");
        }

        static void Ex16() {
            /*
             * If you do not want to call the Task.Wait 
             * or Wait method to wait for a task's completion, 
             * you can also retrieve the AggregateException 
             * exception from the task's Exception property, 
             * as the following example shows.
             */

            var task1 = Task.Run(() => { throw new ArgumentException("This exception is expected!"); });

            while (!task1.IsCompleted) { }

            if (task1.Status == TaskStatus.Faulted) {
                foreach (var e in task1.Exception.InnerExceptions) {
                    // Handle the custom exception.
                    if (e is ArgumentException) {
                        Console.WriteLine(e.Message);
                    }
                    // Rethrow any other exception.
                    else {
                        throw e;
                    }
                }
            }
        }

        static void Ex15() {
            /*
             * Unhandled exceptions that are thrown 
             * by user code that is running inside 
             * a task are propagated back to the calling thread.
             * Exceptions are propagated when 
             * you use one of the static 
             * or instance Task.Wait or Wait methods, 
             * and you handle them by enclosing 
             * the call in a try/catch statement.
             */

            var task1 = Task.Run(() => { throw new ArgumentException("This exception is expected!"); });

            try {
                task1.Wait();
            } catch (AggregateException ae) {
                foreach (var e in ae.InnerExceptions) {
                    // Handle the custom exception.
                    if (e is ArgumentException) {
                        Console.WriteLine(e.Message);
                    }
                    // Rethrow any other exception.
                    else {
                        throw;
                    }
                }
            }
        }

        static void Ex06() {
            //you can pass data to a task by using 
            //the StartNew overloads that accepts Action<object>
            Task t = Task.Factory.StartNew(data => Console.WriteLine($"{data}"), "some value");
        
            //This state is passed as an argument to the task delegate
            //and it can be accessed from the task object by using the Task.AsyncState property.
            Console.WriteLine(t.AsyncState); 
        }

        static void Ex05() {
            //you can pass data to a task by using 
            //the StartNew overloads that accepts Action<object>
            Task.Factory.StartNew(data => Console.WriteLine($"{data}"), "some value");
        }

        static void Ex04() {
            //A task that returns a value is represented 
            //by the System.Threading.Tasks.Task<TResult> 
            //class, which inherits from Task.

            Task<int> taskA = Task.Run(() => {
                Console.WriteLine($"Hello from the task - ThreadId: {Thread.CurrentThread.ManagedThreadId}");
                return 5;
            });

            //If the Result property is accessed 
            //before the computation finishes, 
            //the property blocks the calling thread 
            //until the value is available.
            Console.WriteLine(taskA.Result);
        }

        static void Ex03() {
            //You can also use the Task.Factory.StartNew method 
            //to create and start a task in one operation.
            Task taskA = Task.Factory.StartNew(() => Console.WriteLine($"Hello from the task - ThreadId: {Thread.CurrentThread.ManagedThreadId}"));
        }
        static void Ex02() {
            //You can also use the Task.Run methods 
            //to create and start a task in one operation. T
            Task taskA = Task.Run(() => Console.WriteLine($"Hello from the task - ThreadId: {Thread.CurrentThread.ManagedThreadId}"));
        }

        static void Ex01() {
            //A task that does not return a value is represented 
            //by the System.Threading.Tasks.Task class. 
            //When you create a task, you give it a user delegate 
            //that encapsulates the code that the task will execute. 
            Task taskA = new Task(() => Console.WriteLine($"Hello from taskA - ThreadId: {Thread.CurrentThread.ManagedThreadId}")); 
            // Start the task.
            taskA.Start();
            Console.WriteLine($"Back on Main ThreadId : {Thread.CurrentThread.ManagedThreadId}");
            taskA.Wait(); //blocks
            Console.WriteLine($"taskA ended - ThreadId : {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
