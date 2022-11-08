using SlowLibrary;

public class Program {
    public static void Main() {

    }

    /// <summary>
    /// Invoke the SlowMethod01 3 times in a synchronous, single threaded way
    /// </summary>
    static void Lab00() {
        SlowClass sc = new SlowClass();
        sc.SlowMethod01();
        sc.SlowMethod01();
        sc.SlowMethod01();

        Console.WriteLine("Lab00 is done");
    }

    /// <summary>
    /// Invoke the SlowMethod01 3 times using 3 parallel Threads
    /// </summary>
    static void Lab01() {
        SlowClass sc = new SlowClass();
        new Thread(sc.SlowMethod01).Start();
        new Thread(sc.SlowMethod01).Start();
        new Thread(sc.SlowMethod01).Start();

        Console.WriteLine("Lab01 is done");
    }

    /// <summary>
    /// Invoke the SlowMethod02 3 times using 3 parallel Threads,
    /// passing 4, 3 and 5
    /// </summary>
    static void Lab02() {
        SlowClass sc = new SlowClass();
        new Thread(sc.SlowMethod02).Start(4);
        new Thread(sc.SlowMethod02).Start(3);
        new Thread(sc.SlowMethod02).Start(5);

        Console.WriteLine("Lab02 is done");
    }

    /// <summary>
    /// Invoke the SlowMethod03 3 times using 3 parallel Threads,
    /// passing 4, 3 and 5
    /// </summary>
    static void Lab03() {
        SlowClass sc = new SlowClass();
        new Thread(() => sc.SlowMethod03(4)).Start();
        new Thread(() => sc.SlowMethod03(3)).Start();
        new Thread(() => sc.SlowMethod03(5)).Start();

        Console.WriteLine("Lab03 is done");
    }

    ///<summary>
    /// Invoke the SlowMethod04 3 times using 3 parallel Threads,
    /// passing 4, 3 and 5. 
    /// Store the 3 results in a sum variable
    /// and show the sum after the 3 threads have completed.
    /// Don't forget to update the sum in a thread safe way.
    /// </summary>
    static void Lab04() {
        SlowClass sc = new SlowClass();

        int sum = 0;
        object lockOnSum = new object();
        Thread t1 = new Thread(() => {
            int result = sc.SlowMethod04(4);
            lock (lockOnSum) {
                sum += result;
            }
            Console.WriteLine($"The square of {4} is {result}");
        });
        t1.Start();

        Thread t2 = new Thread(() => {
            int result = sc.SlowMethod04(3);
            lock (lockOnSum) {
                sum += result;
            }
            Console.WriteLine($"The square of {3} is {result}");
        });
        t2.Start();

        Thread t3 = new Thread(() => {
            int result = sc.SlowMethod04(5);
            lock (lockOnSum) {
                sum += result;
            }
            Console.WriteLine($"The square of {5} is {result}");
        });
        t3.Start();

        t1.Join();
        t2.Join();
        t3.Join();

        Console.WriteLine($"The sum is {sum}");
    }

    ///<summary>
    /// Repeat the previous exercise, but this time use a for loop to start the 3 threads.
    /// Watch out to not use the shared loop index inside the lambda expression
    /// or you may have race conditions.
    /// </summary>
    static void Lab05() {
        SlowClass sc = new SlowClass();
        int sum = 0;

        int[] numbers = { 4, 3, 5 };
        Thread[] threads = new Thread[3];

        for (int i = 0; i < numbers.Length; i++) {
            //int index = i;
            threads[i] = new Thread((index) => {
                int result = sc.SlowMethod04(numbers[(int)index]);
                Interlocked.Add(ref sum, result);
                Console.WriteLine($"The square of {numbers[(int)index]} is {result}");
            });
            threads[i].Start(i);

        }
        Console.WriteLine($"The sum is {sum}");
        for (int i = 0; i < threads.Length; i++) {
            threads[i].Join();
        }

        Console.WriteLine($"The sum is {sum}");
    }
}