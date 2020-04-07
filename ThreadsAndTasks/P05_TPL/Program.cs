using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace P05_TPL
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/data-parallelism-task-parallel-library
             * http://www.albahari.com/threading/part5.aspx#_The_Parallel_Class
             *                                                                                        T1
             * PFX provides a basic form of structured parallelism                                    |
             * via three static methods in the Parallel class:                                        |
                                                                                                     FORK
                Parallel.Invoke                                                                       ^
                Executes an array of delegates in parallel                                           / \
                                                                                                     |  |
                Parallel.For                                                                        T2  T3
                Performs the parallel equivalent of a C# for loop                                    \  /
                                                                                                      V
                Parallel.ForEach                                                                     JOIN 
                Performs the parallel equivalent of a C# foreach loop                                 |
                                                                                                      |
                All three methods block until all work is complete. 
                As with PLINQ, after an unhandled exception, 
                remaining workers are stopped after their current iteration 
                and the exception (or exceptions) are thrown back to the caller — 
                wrapped in an AggregateException.
                */
            Ex07();
            Console.WriteLine("Press Enter to Exit");
            Console.ReadLine();
        }

        private static void Ex10() {
            void action(int i) {
                Console.WriteLine($"ThreadId: {Thread.CurrentThread.ManagedThreadId} TaskId: {Task.CurrentId} i: {i}");
                if (i % 25 == 0)
                    throw new ArgumentException($"i cannot be {i}");
            }
            
            try {
                Parallel.For(0, 1000, action);
            } catch (Exception ex) {
                Console.WriteLine($"{ex.GetType().Name} - {ex.Message}");
            }
        }

        static void Ex09() {
            /* This example shows how to use thread-local variables 
                to store and retrieve state in each separate task that is created by a For loop. 
                By using thread-local data, you can avoid the overhead of synchronizing 
                a large number of accesses to shared state. 
                Instead of writing to a shared resource on each iteration, 
                you compute and store the value until all iterations 
                for the task are complete. 
                You can then write the final result once to the shared resource, 
                or pass it to another method.
            */
            int init() {
                //initialization of the local state (per chunk)
                Console.WriteLine($"INIT - ThreadId: {Thread.CurrentThread.ManagedThreadId} TaskId: {Task.CurrentId}");
                return 0;
            }

            int action(int i, ParallelLoopState state, int local) {
                //actual loop
                Console.WriteLine($"\tBODY - ThreadId: {Thread.CurrentThread.ManagedThreadId} TaskId: {Task.CurrentId} i: {i} - local: {local}");
                return local + i;
            }

            void final(int local) {
                //subtotal (per chunk)
                Console.WriteLine($"\t\tFINAL - ThreadId: {Thread.CurrentThread.ManagedThreadId} TaskId: {Task.CurrentId} - local: {local}");
            }

            Parallel.For(0, 1000, init, action, final);
        }

        static void Ex08() {
            /*
             * ParallelLoopState: Breaking early out of loops
             * Because the loop body in a parallel For or ForEach 
             * is a delegate, you can’t exit the loop early 
             * with a break statement. 
             * Instead, you must call Break or Stop on a ParallelLoopState object:
             */

            var result = Parallel.For(0, 1000, (i, loopState) => {
                if (i == 130) {
                    Console.WriteLine($"Stop on {i}");
                    loopState.Stop();
                } else
                    Console.WriteLine($"{i} - {Thread.CurrentThread.ManagedThreadId} {Task.CurrentId}");
            });
            Console.WriteLine($"IsCompleted: {result.IsCompleted}");

        }

        static void Ex07() {
            /*
             * ParallelLoopState: Breaking early out of loops
             * Because the loop body in a parallel For or ForEach 
             * is a delegate, you can’t exit the loop early 
             * with a break statement. 
             * Instead, you must call Break or Stop on a ParallelLoopState object:
             */

            var result = Parallel.For(0, 1000, (i, loopState) => {
                if (i == 130) {
                    Console.WriteLine($"Break on {i}");
                    loopState.Break();
                } else
                    Console.WriteLine($"{i} - {Thread.CurrentThread.ManagedThreadId} {Task.CurrentId}");
            });
            Console.WriteLine($"IsCompleted: {result.IsCompleted} LowestBreakIteration: {result.LowestBreakIteration}");

        }

        static void Ex06() {
            /*
             * Sometimes it’s useful to know the loop iteration index. 
             * You must use the following version of ForEach:
             * The third type parameter of type long indicates the loop index
             */

            string sequence = "supercalifragilisticexpialidocious";

            void action(char c, ParallelLoopState state, long index) {
                Console.WriteLine($"{c} - index: {index} - ThreadId: {Thread.CurrentThread.ManagedThreadId}");
            } 

            Parallel.ForEach(sequence, action);
            Console.WriteLine($"After For, back on ThreadId: {Thread.CurrentThread.ManagedThreadId}");
        }

        static void Ex05() {
            /*
             * Parallel.ForEach performs the equivalent 
             * of a C# foreach loop, but with each iteration 
             * executing in parallel instead of sequentially.
             */
            string sequence = "supercalifragilisticexpialidocious";

            void action (char c) {
                Console.WriteLine($"{c} - ThreadId: {Thread.CurrentThread.ManagedThreadId}");
            }

            Parallel.ForEach(sequence, action);
            Console.WriteLine($"After For, back on ThreadId: {Thread.CurrentThread.ManagedThreadId}");
        }

        static void Ex04() {
            /*
             * Parallel.For performs the equivalent 
             * of a C# for loop, but with each iteration 
             * executing in parallel instead of sequentially.
             */

            Parallel.For(0, 100, i => Console.WriteLine($"{i} - ThreadId: {Thread.CurrentThread.ManagedThreadId}"));
            Console.WriteLine($"After For, back on ThreadId: {Thread.CurrentThread.ManagedThreadId}");
        }

        static void Ex03() {
            /*
             * With ParallelOptions, you can insert a cancellation token. 
             * A cancellation token is relevant when you’re executing (roughly) 
             * more tasks than you have cores: upon cancellation, 
             * any unstarted delegates will be abandoned. 
             * Any already-executing delegates will, however, continue to completion. 
             */
            CancellationTokenSource cts = new CancellationTokenSource();

            void print() {
                if (cts.Token.IsCancellationRequested)
                    cts.Token.ThrowIfCancellationRequested();
                Console.WriteLine($"ThreadId: {Thread.CurrentThread.ManagedThreadId} - TaskId: {Task.CurrentId}");
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

            Action[] actions = Enumerable.Repeat<Action>(print, 100000).ToArray();

            try {
                Parallel.Invoke(new ParallelOptions() { CancellationToken = cts.Token }, actions);
            }catch(Exception ex){
                Console.WriteLine($"EXCEPTION!: {ex}");
            }

            Console.WriteLine("Ex03 done");
        }

        static void Ex02() {
            /*
             * Parallel.Invoke still works efficiently if you pass in 
             * an array of a million delegates. This is because it partitions 
             * large numbers of elements into batches which it assigns 
             * to a handful of underlying Tasks — rather than creating 
             * a separate Task for each delegate.
             */

            void print() {
                Console.WriteLine($"Thread Id: {Thread.CurrentThread.ManagedThreadId} - Task Id:{Task.CurrentId}");
            }

            Action[] actions = Enumerable.Repeat<Action>(print, 1000).ToArray();

            Parallel.Invoke(actions);

            //we automatically join all threads: we're back on thread 1 here
            print();
        }

        static void Ex01() {
            void print() {
                Console.WriteLine($"Thread Id: {Thread.CurrentThread.ManagedThreadId} - Task Id:{Task.CurrentId}");
            }
            /*
             * Parallel.Invoke executes an array of Action delegates in parallel, 
             * then waits for them to complete. 
             */

            Parallel.Invoke(print,print);

            //we automatically join both threads: we're back on thread 1 here
            print();
        }
    }
}
