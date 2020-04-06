using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace P07_TaskBasedAsynchronousPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            //https://docs.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/task-based-asynchronous-pattern-tap

            Ex18();
            Console.WriteLine("Press Enter To Exit!");
            Console.ReadLine();
        }

        //https://devblogs.microsoft.com/pfxteam/processing-tasks-as-they-complete/
        static async Task Ex18() {
            //this is a version of Ex17 that uses TaskCompletionSources 
            //to return Tasks as soon as they complete.
            async Task<int> SlowPower(int n) {
                await Task.Delay(n * 1000);
                return n * n;
            }

            async IAsyncEnumerable<int> SlowPowers(int[] numbers) {
                List<Task<int>> tasks = numbers.Select(SlowPower).ToList();
                List<TaskCompletionSource<int>> completedTasks = new List<TaskCompletionSource<int>>();
                for (int i = 0; i < numbers.Length; i++) {
                    completedTasks.Add(new TaskCompletionSource<int>());
                }

                int index = -1;
                foreach (var item in tasks) {
                    item.ContinueWith(t => {
                       completedTasks[Interlocked.Increment(ref index)].SetResult(t.Result);
                    });
                }

                foreach (var item in completedTasks) {
                    yield return await item.Task;
                }
            }

            int[] list = new int[] { 3, 11, 5, 4, 10, 5, 8, 6 };

            await foreach (var item in SlowPowers(list)) {
                Console.WriteLine($"The square is {item}");
            }
           
        }

        //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/start-multiple-async-tasks-and-process-them-as-they-complete
        static async Task Ex17() {
            //this is a version of Ex16 where
            //we return the results as soon as we get them,
            //instead of in the same order as we got the input
            async Task<int> SlowPower(int n) {
                await Task.Delay(n * 1000);
                return n * n;
            }

            async IAsyncEnumerable<int> SlowPowers(int[] numbers) {
                //first of all we invoke SlowPower in parallel, without waiting
                List<Task<int>> tasks = numbers.Select(SlowPower).ToList();

                //then we wait for the first completed task in the list 
                //(thanks to Task.WhenAny) and return the result.
                //after having removed the completed task from the list,
                //we wait for the next and so on.

                //Microsoft says that this is an option that 
                //costs a lot of performances,
                //so it's ok if the list of tasks is not too big,
                //but it's not optimal
                while (tasks.Count > 0) { 
                    var completedTask = await Task.WhenAny(tasks);
                    tasks.Remove(completedTask);
                    yield return completedTask.Result;
                }
            }

            int[] list = new int[] { 3, 2, 5, 4, 7, 5, 8, 6 };
            await foreach (var item in SlowPowers(list)) {
                Console.WriteLine($"The square is {item}");
            }
        }

        static async Task Ex16() {
            //this is a better version of ex15.

            async Task<int> SlowPower(int n) {
                await Task.Delay(n * 1000);
                return n * n;
            }

            async IAsyncEnumerable<(int, int)> SlowPowers(int[] numbers) {
                //what's better is the fact that the SlowPower is invoked
                //for each item od the input array
                //without waiting, which means that all the results
                //are calculated in parallel
                Task<int>[] tasks = numbers.Select(SlowPower).ToArray();

                //but we still wait for each result
                //before going to the next, which means
                //that if the next value is faster than the previous,
                //we still wait for the slowest before proceeding
                //to the result that was already there.
                //The results are given in the same order as the input,
                //which could be what we want.
                for (int i = 0; i < numbers.Length; i++) {
                    yield return (numbers[i], await tasks[i]);
                }
            }

            int[] list = new int[] { 3, 2, 5, 4, 7, 5, 8, 6 };
            await foreach (var item in SlowPowers(list)) {
                Console.WriteLine($"The square of {item.Item1} is {item.Item2}");
            }
        }

        static async Task Ex15() { 
            //C# 8 introduces IAsyncEnumerable
            //that can be used in conjunction with yield return
            //and await foreach
            async Task<int> SlowPower(int n) {
                await Task.Delay(n * 1000);
                return n * n;
            }

            // the SlowPowers method returns an async stream of tuples.
            // as soon as the next value is available, it is returned,
            // one by one
            async IAsyncEnumerable<(int, int)> SlowPowers(int[] numbers) {
                for (int i = 0; i < numbers.Length; i++) {
                    yield return (numbers[i], await SlowPower(numbers[i]));
                }    
            }

            int[] list = new int[] {3,2,5,4,7,5,8,6 };
            await foreach (var item in SlowPowers(list)) {
                Console.WriteLine($"The square of {item.Item1} is {item.Item2}");
            }
        }

        static async void Ex14() {
            //await all the things!
            //You can await on any instance of a class
            //that contains a GetAwaiter method.
            //Thanks to Extension Methods, you can plug
            //a GetAwaiter method to anything.
            Process p = Process.Start("notepad");
            await p;
            Console.WriteLine("notepad is done!");
        }

        static async void Ex13() {
            async Task<int> funcAsync(int seconds){
                Console.WriteLine($"funcAsync started on Thread {Thread.CurrentThread.ManagedThreadId}");
                await Task.Delay(seconds * 1000);
                return seconds;
            };

            async Task awaiter(Task<int> t){
                Console.WriteLine($"done with a result of {await t} on Thread {Thread.CurrentThread.ManagedThreadId}");
            };

            int[] parameters = { 1, 3, 2 };

            //this starts calling all the tasks 
            //without waiting for them to finish
            //and gives us back all the returned tasks
            var tasks = parameters.Select(funcAsync).ToArray();

            //this starts waiting for every task as soon as possible
            //and gives us back all the tasks
            var results = tasks.Select(awaiter).ToArray();

            //we can wait for all of the handling to be complete
            //if we want
            await Task.WhenAll(results);
            Console.WriteLine($"done on ThreadId {Thread.CurrentThread.ManagedThreadId}");
        }

        static void Ex12() {
            //the await unwraps the Task.Result

            async Task<int> funcAsync(int seconds){
                Console.WriteLine($"funcAsync started on Thread {Thread.CurrentThread.ManagedThreadId}");
                await Task.Delay(seconds * 1000);
                return seconds;
            };

            //if we don't await, we continue
            Task<int> t1 = funcAsync(3);
            Task<int> t2 = funcAsync(2);
            Task<int> t3 = funcAsync(4);

            //the trick to retrieve the results as soon as possible is to await
            //in a different method.
            //This way the method yields back to the caller
            //so that the caller can continue further
            async Task awaiter(Task<int> t){
                Console.WriteLine($"done with a result of {await t} on Thread {Thread.CurrentThread.ManagedThreadId}");
            };

            //we do not await, so we're fast here
            awaiter(t1);
            awaiter(t2);
            awaiter(t3);
        }

        static async void Ex11() {
            async Task<int> funcAsync(int seconds){
                Console.WriteLine($"funcAsync started on Thread {Thread.CurrentThread.ManagedThreadId}");
                await Task.Delay(seconds * 1000);
                return seconds;
            };
            //if we don't await, we continue
            Task<int> t1 = funcAsync(3);
            Task<int> t2 = funcAsync(2);
            Task<int> t3 = funcAsync(4);

            t1.ContinueWith(t => Console.WriteLine($"t1 done with a result of {t.Result} on Thread {Thread.CurrentThread.ManagedThreadId}"));
            t2.ContinueWith(t => Console.WriteLine($"t2 done with a result of {t.Result} on Thread {Thread.CurrentThread.ManagedThreadId}"));
            t3.ContinueWith(t => Console.WriteLine($"t3 done with a result of {t.Result} on Thread {Thread.CurrentThread.ManagedThreadId}"));

            int[] results = await Task.WhenAll(t1, t2, t3);

            Console.WriteLine($"done on Thread {Thread.CurrentThread.ManagedThreadId}");
        }

        static void Ex10() {
            //the await unwraps the Task.Result

            async Task<int> funcAsync(int seconds){
                Console.WriteLine($"funcAsync started on Thread {Thread.CurrentThread.ManagedThreadId}");
                await Task.Delay(seconds * 1000);
                return seconds;
            };

            //if we don't await, we continue
            Task<int> t1 = funcAsync(3);
            Task<int> t2 = funcAsync(2);
            Task<int> t3 = funcAsync(4);

            //we can still await and get the result, 
            //even if the result is already complete
            Task.Run(async () => Console.WriteLine($"t1 done with a result of {await t1} on Thread {Thread.CurrentThread.ManagedThreadId}"));
            Task.Run(async () => Console.WriteLine($"t2 done with a result of {await t2} on Thread {Thread.CurrentThread.ManagedThreadId}"));
            Task.Run(async () => Console.WriteLine($"t3 done with a result of {await t3} on Thread {Thread.CurrentThread.ManagedThreadId}"));
        }

        static async void Ex09() {
            //the await unwraps the Task.Result

            async Task<int> funcAsync(int seconds){
                Console.WriteLine($"funcAsync started on Thread {Thread.CurrentThread.ManagedThreadId}");
                await Task.Delay(seconds * 1000);
                return seconds;
            };
            //if we don't await, we continue
            Task<int> t1 = funcAsync(3);
            Task<int> t2 = funcAsync(2);
            Task<int> t3 = funcAsync(4);

            //we can still await and get the result, 
            //even if the result is already complete

            Console.WriteLine($"t1 done with a result of {await t1} on Thread {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine($"t2 done with a result of {await t2} on Thread {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine($"t3 done with a result of {await t3} on Thread {Thread.CurrentThread.ManagedThreadId}");
        }

        static async void Ex08() {
            //the await unwraps the Task.Result

            async Task<int> funcAsync(){
                await Task.Delay(1000);
                return 10;
            };

            int result = await funcAsync();
            Console.WriteLine($"done with a result of {result} on Thread {Thread.CurrentThread.ManagedThreadId}");
        }

        static void Ex07() {
            /* The await keyword is where the magic happens. 
             * It yields control to the caller of the method 
             * that performed await, and it ultimately allows 
             * a UI to be responsive or a service to be elastic.
             */

            async Task funcAsync(){
                Console.WriteLine($"funcAsync is starting on Thread {Thread.CurrentThread.ManagedThreadId}");
                await Task.Run(() => {
                    Console.WriteLine($"task running on Thread {Thread.CurrentThread.ManagedThreadId}...");
                    for (long i = 0; i < 1000000000; i++) {

                    }
                });
                Console.WriteLine($"done on Thread {Thread.CurrentThread.ManagedThreadId}");
            };

            funcAsync();
            Console.WriteLine($"in the meantime I'm free to continue on Thread {Thread.CurrentThread.ManagedThreadId}");
        }

        static void Ex06() {
            /* Starting with .NET Framework 4.5, 
             * any method that is attributed with the async keyword 
             * is considered an asynchronous method, and the C# compiler 
             * performs the necessary transformations to implement 
             * the method asynchronously by using TAP. 
             * An asynchronous method should return 
             * either a System.Threading.Tasks.Task 
             * or a System.Threading.Tasks.Task<TResult> object. 
             * For the latter, the body of the function should return a TResult, 
             * and the compiler ensures that this result is made available 
             * through the resulting task object. 
             */

            async Task funcAsync(){
                Console.WriteLine($"I'm an async method on Thread {Thread.CurrentThread.ManagedThreadId}!");
                //the compiler returns a Task for us
            };

            async Task<int> funcAsync2(){
                Console.WriteLine($"I'm an async method on Thread {Thread.CurrentThread.ManagedThreadId}!");
                //the compiler returns a Task<int> for us
                return 42;
            };

            funcAsync().ContinueWith(t => Console.WriteLine($"funcAsync is done on Thread {Thread.CurrentThread.ManagedThreadId}"));

            funcAsync2().ContinueWith(t => Console.WriteLine($"funcAsync2 is done with {t.Result} on Thread {Thread.CurrentThread.ManagedThreadId}"));
            Console.WriteLine($"in the meantime I'm going on on Thread {Thread.CurrentThread.ManagedThreadId}");
        }

        static void Ex05() {
            /*
             * Some asynchronous operations benefit from providing progress notifications; 
             * these are typically used to update a user interface with information about 
             * the progress of the asynchronous operation.
             * 
             * In TAP, progress is handled through an IProgress<T> interface, 
             * which is passed to the asynchronous method as a parameter 
             * that is usually named progress
             */

            Task<int> funcAsync(IProgress<int> progress){
                for (int workComplete = 0; workComplete < 100; workComplete++) {
                    for (int i = 0; i < 100000000; i++) {
                        //doing hard work here...
                    }
                    progress.Report(workComplete);
                }
                return Task.FromResult(42);
            };

            /* An instance of Progress<T> exposes a ProgressChanged event, 
             * which is raised every time the asynchronous operation 
             * reports a progress update. 
             */

            Progress<int> p = new Progress<int>();
            p.ProgressChanged += (source, percentComplete) => {
                Console.WriteLine($"{percentComplete}% complete");
            };

            funcAsync(p).ContinueWith(t => Console.WriteLine($"done! {t.Result}"));
        }

        static void Ex04() {
            //You can also use Task.FromResult to return a precomputed Task

            //Let's pretend you have a method that may retrieve a value
            //asynchronously but it may also already 
            //have a result to return immediately.
            //If you say that your method returns a Task, 
            //then you HAVE to return a task, but how do you return
            //a value that you already have?
            //You can do it with Task.FromResult(yourValue)
            Task<int> funcAsync(){
                //I check if I have the value.
                //If I don't, I start a Task to 
                //perform an operation asynchronously
                //that will eventually return a value.
                //But if I already have the value,
                //I just return it like this:
                return Task.FromResult(42);
            };

            funcAsync().ContinueWith(t => Console.WriteLine($"done with a result of {t.Result} on Thread {Thread.CurrentThread.ManagedThreadId}"));
        }

        static void Ex03() {
            //another example of TaskCompletionSource
            Task<Process> runProcessAsync(string fileName){
                TaskCompletionSource<Process> taskCompletionSource = new TaskCompletionSource<Process>();
                Process process = new Process() {
                    StartInfo = new ProcessStartInfo(fileName) {
                        UseShellExecute = false
                    },
                    EnableRaisingEvents = true,
                };

                process.Exited += (sender, localEventArgs) => {
                    taskCompletionSource.SetResult(process);
                };

                process.Start();

                return taskCompletionSource.Task;
            };

            runProcessAsync("notepad").ContinueWith(t => {
                Console.WriteLine("notepad is done!");
            });

            Console.WriteLine($"in the meantime I can do my own things on Thread {Thread.CurrentThread.ManagedThreadId}...");
        }

        static void Ex02() {
            /*
             * To implement the TAP yourself, 
             * you create a TaskCompletionSource<TResult> object, 
             * perform the asynchronous operation, 
             * and when it completes, call the 
             * SetResult, SetException, or SetCanceled method, 
             * or the Try version of one of these methods. 
             * When you implement a TAP method manually, 
             * you must complete the resulting task 
             * when the represented asynchronous operation completes. 
             */

            Task funcAsync(){
                TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
                Timer t = new Timer(_ => { tcs.SetResult(true); }, null, 2000, Timeout.Infinite);
                return tcs.Task;
            };

            funcAsync().ContinueWith(t => {
                Console.WriteLine($"done on Thread {Thread.CurrentThread.ManagedThreadId}");
            });

            Console.WriteLine($"in the meantime I'm free to do my stuff on Thread {Thread.CurrentThread.ManagedThreadId}");
        }

        static void Ex01() {
            /* A TAP method returns either a System.Threading.Tasks.Task 
             * or a System.Threading.Tasks.Task<TResult>, 
             * based on whether the corresponding synchronous 
             * method returns void or a type TResult.
             */

            //this function is very slow
            int calculateSlow(int input) {
                for (long i = 0; i < 1000000000 * input; i++) { }
                return input * input;
            };

            //let's create the async version
            Task<int> calculateSlowAsync(int input){
                //we could do some synchronous work,
                //such as checking if the input is valid,
                //then we start the async operation and return 
                //the running task
                return Task.Run(() => {
                    return calculateSlow(input);
                });
            };

            calculateSlowAsync(10).ContinueWith(t => Console.WriteLine($"done with a result of {t.Result} on Thread {Thread.CurrentThread.ManagedThreadId}"));

            Console.WriteLine($"in the meantime I'm free to go on on Thread {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
