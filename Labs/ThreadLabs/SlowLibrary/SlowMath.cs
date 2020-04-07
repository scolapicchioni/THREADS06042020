using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SlowLibrary {
    public class SlowMath {
        public void SlowSquare01(object o) {
            int n = (int)o;
            for (int i = 0; i < 1_000_000; i++) {

            }
            Console.WriteLine(n * n);
        }

        public static void SlowSquare02(object o) {
            int n = (int)o;
            for (int i = 0; i < 1_000_000; i++) {

            }
            Console.WriteLine(n * n);
        }

        public static void SlowSquare03(int n) {
            for (int i = 0; i < 1_000_000; i++) {

            }
            Console.WriteLine(n * n);
        }

        public static int SlowSquare04(int n) {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < (n * 1000)) { 
            }
            return n * n;
        }

        public static Task<int> SlowSquareAsync(int n)
        {
            return Task.Run(()=>SlowSquare04(n));
        }

    }
}
