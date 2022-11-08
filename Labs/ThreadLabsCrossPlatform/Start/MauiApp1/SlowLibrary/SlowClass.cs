using System.Diagnostics;

namespace SlowLibrary {
    public class SlowClass {
        //lab 01: run this method 3 times in parallel
        public void SlowMethod01() { 
            Stopwatch sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 3_000) { }
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} - Done after 3 seconds");
        }
        //lab 02: run this method 3 times in parallel
        //passing 4, 3 and 5 
        public void SlowMethod02(object seconds) {
            Stopwatch sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < (int)seconds * 1_000) { }
            Console.WriteLine($"Done after {seconds} seconds");
        }
        //lab 03: run this method 3 times in parallel
        //passing 4, 3 and 5 
        public void SlowMethod03(int seconds) {
            Stopwatch sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < seconds * 1_000) { }
            Console.WriteLine($"Done after {seconds} seconds");
        }

        //lab 04: run this method 3 times in parallel and print the result
        //passing 4, 3 and 5 
        //at the end, write the sum of the tree results
        public int SlowMethod04(int seconds) {
            Stopwatch sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < seconds * 1_000) { }
            return seconds * seconds;
        }
    }
}