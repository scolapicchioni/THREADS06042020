using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SlowLibrary {
    public class SlowMath {
        public void SlowFunction() {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < 3000) { }
            Console.WriteLine(42);
        }
        public void SlowSquare01(object o) {
            int n = (int)o;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < (n * 1000))
            {
            }
            Console.WriteLine(n * n);
        }

        public static void SlowSquare02(object o) {
            int n = (int)o;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < (n * 1000))
            {
            }
            Console.WriteLine(n * n);
        }

        public void SlowSquare02(int n) {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < (n * 1000))
            {
            }
            Console.WriteLine(n * n);
        }

        public int SlowSquare03(int n) {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < (n * 1000)) {
            }
            return n * n;
        }

        public int Sum { get; private set; }
        public object lockOnSum = new object();
        public void SlowSquare04WithSumUpdate(int n)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < (n * 1000))
            {
            }
            int result = n * n;
            Console.WriteLine($"The square is {result}");
            lock (lockOnSum) {
                Sum += result;
                Console.WriteLine($"The sum is now {Sum}");
            }
        }

        public static int SlowSquare05(int n) {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < (n * 1000)) { 
            }
            return n * n;
        }

        public static Task<int> SlowSquareAsync(int n)
        {
            return Task.Run(()=>SlowSquare05(n));
        }

    }
}
