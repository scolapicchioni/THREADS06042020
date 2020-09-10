using System;
using System.Threading;
using SlowLibrary;

namespace ConsoleApp01 {
    class Program {
        static void Main(string[] args)
        {
            Ex01();
        }

        private static void Ex08()
        {
            int sum = 0;

            EventWaitHandle[] events = new EventWaitHandle[] {
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false)
            };

            int[] values = new int[] { 5, 3, 4 };

            for (int i = 0; i < 3; i++)
            {
                new Thread((myI) =>
                {
                    int result = SlowMath.SlowSquare04(values[(int)myI]);
                    Interlocked.Add(ref sum, result);
                    Console.WriteLine(result);
                    events[(int)myI].Set();
                }).Start(i);
            }

            EventWaitHandle.WaitAll(events);
            Console.WriteLine($"The sum is {sum}");
        }

        private static void Ex07() {
            int sum = 0;
            object lockOnSum = new object();

            EventWaitHandle[] events = new EventWaitHandle[] {
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false)
            };

            int[] values = new int[] { 5, 3, 4 };

            for (int i = 0; i < 3; i++) {
                new Thread((myI) => {
                    int result = SlowMath.SlowSquare04(values[(int)myI]);
                    lock (lockOnSum) {
                        sum += result;
                    }
                    Console.WriteLine(result);
                    events[(int)myI].Set();
                }).Start(i);
            }

            EventWaitHandle.WaitAll(events);
            Console.WriteLine($"The sum is {sum}");
        }

        private static void Ex06() {
            int sum = 0;
            object lockOnSum = new object();

            EventWaitHandle[] events = new EventWaitHandle[] {
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false)
            };

            new Thread(() => {
                int result = SlowMath.SlowSquare04(5);
                lock (lockOnSum) {
                    sum += result;
                }
                Console.WriteLine(result);
                events[0].Set();
            }).Start();

            new Thread(() => {
                int result = SlowMath.SlowSquare04(3);
                lock (lockOnSum) {
                    sum += result;
                }
                Console.WriteLine(result);
                events[1].Set();
            }).Start();

            new Thread(() => {
                int result = SlowMath.SlowSquare04(4);
                lock (lockOnSum) {
                    sum += result;
                }
                Console.WriteLine(result);
                events[2].Set();
            }).Start();

            EventWaitHandle.WaitAll(events);
            Console.WriteLine($"The sum is {sum}");
        }

        private static void Ex05() {
            int sum = 0;
            object lockOnSum = new object();
            new Thread(() => {
                int result = SlowMath.SlowSquare04(5);
                lock (lockOnSum) {
                    sum += result;
                }
                Console.WriteLine(result);
            }).Start();

            new Thread(() => {
                int result = SlowMath.SlowSquare04(3);
                lock (lockOnSum) {
                    sum += result;
                }
                Console.WriteLine(result);
            }).Start();

            new Thread(() => {
                int result = SlowMath.SlowSquare04(4);
                lock (lockOnSum) {
                    sum += result;
                }
                Console.WriteLine(result);
            }).Start();
        }

        private static void Ex04() {
            new Thread(() => {
                int result = SlowMath.SlowSquare04(5);
                Console.WriteLine(result);
            }).Start();

            new Thread(() => {
                int result = SlowMath.SlowSquare04(3);
                Console.WriteLine(result);
            }).Start();

            new Thread(() => {
                int result = SlowMath.SlowSquare04(4);
                Console.WriteLine(result);
            }).Start();
        }

        private static void Ex03() {
            new Thread(() => SlowMath.SlowSquare03(5)).Start();
            new Thread(() => SlowMath.SlowSquare03(6)).Start();
            new Thread(() => SlowMath.SlowSquare03(7)).Start();
        }

        private static void Ex02() {
            new Thread(SlowMath.SlowSquare02).Start(5);
            new Thread(SlowMath.SlowSquare02).Start(6);
            new Thread(SlowMath.SlowSquare02).Start(7);
        }

        private static void Ex01() {
            SlowMath sm = new SlowMath();
            sm.SlowSquare01(4);
            sm.SlowSquare01(5);
            sm.SlowSquare01(3);

            Thread t1 = new Thread(sm.SlowSquare01);
            Thread t2 = new Thread(sm.SlowSquare01);
            Thread t3 = new Thread(sm.SlowSquare01);

            t1.Start(4);
            t2.Start(5);
            t3.Start(3);
        }
    }
}
