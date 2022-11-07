using SlowLibrary;
namespace MauiApp1;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    /// <summary>
    /// Invoke SlowMethod4 in a synchronous, single threaded way,
    /// passing the values of the 3 Entry.
    /// Write the results on the 3 Labels and the sum on lblSum.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
	private void btnSingleThreaded_Click(object sender, EventArgs e)
	{
        SlowClass sc = new SlowClass();
        int sum = 0;

        int[] numbers = { int.Parse(txtNumber01.Text), int.Parse(txtNumber02.Text), int.Parse(txtNumber03.Text) };
        Label[] labels = { lblResult01, lblResult02, lblResult03 };

        foreach (Label label in labels) {
            label.Text = string.Empty;
        }
        lblSum.Text = string.Empty;

        for (int i = 0; i < numbers.Length; i++) {
            int result = sc.SlowMethod04(numbers[i]);
            sum += result;
            labels[i].Text = result.ToString();
        }

        lblSum.Text = sum.ToString();
	}

    /// <summary>
    /// Invoke SlowMethod4 using 3 parallel Threads,
    /// passing the values of the 3 Entry.
    /// Write the results on the 3 Labels.
    /// Use a SynchronizationContext to get back to 
    /// the Thread that owns your labels.
    /// Join the threads to wait for the sum and update labelSum with the result.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnThreads_Click(object sender, EventArgs e) {
        SynchronizationContext contextOfMainThread = SynchronizationContext.Current;
        SlowClass sc = new SlowClass();
        int sum = 0;
        int[] numbers = { int.Parse(txtNumber01.Text), int.Parse(txtNumber02.Text), int.Parse(txtNumber03.Text) };
        Thread[] threads = new Thread[3];
        Label[] labels = { lblResult01, lblResult02, lblResult03 };

        foreach (Label label in labels) {
            label.Text = string.Empty;
        }
        lblSum.Text = string.Empty;


        for (int i = 0; i < numbers.Length; i++) {
            int index = i;
            threads[i] = new Thread(() => {
                int result = sc.SlowMethod04(numbers[index]);
                Interlocked.Add(ref sum, result);
                contextOfMainThread.Post(_ => labels[index].Text = result.ToString(), null);
            });
            threads[i].Start();
        }
        new Thread(() => {
            for (int i = 0; i < threads.Length; i++) {
                threads[i].Join();
            }
            contextOfMainThread.Post(_ => lblSum.Text = sum.ToString(), null);
        }).Start();
    }

    /// <summary>
    /// Invoke SlowMethod4 using 3 parallel Threads,
    /// passing the values of the 3 Entry.
    /// Write the results on the 3 Labels.
    /// Use a SynchronizationContext to get back to 
    /// the Thread that owns your labels.
    /// Use a CountdownEvent to wait for the sum and update labelSum with the result.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCountdown_Click(object sender, EventArgs e) {
        SynchronizationContext contextOfMainThread = SynchronizationContext.Current!;
        SlowClass sc = new SlowClass();
        int sum = 0;
        int[] numbers = { int.Parse(txtNumber01.Text), int.Parse(txtNumber02.Text), int.Parse(txtNumber03.Text) };
        Thread[] threads = new Thread[3];
        Label[] labels = { lblResult01, lblResult02, lblResult03 };

        foreach (Label label in labels) {
            label.Text = string.Empty;
        }
        lblSum.Text = string.Empty;

        CountdownEvent countdownEvent = new CountdownEvent(3);
        for (int i = 0; i < numbers.Length; i++) {
            int index = i;
            threads[i] = new Thread(() => {
                int result = sc.SlowMethod04(numbers[index]);
                Interlocked.Add(ref sum, result);
                countdownEvent.Signal();
                contextOfMainThread.Post(_ => labels[index].Text = result.ToString(), null);
            });
            threads[i].Start();
        }
        new Thread(() => {
            countdownEvent.Wait();
            contextOfMainThread.Post(_ => lblSum.Text = sum.ToString(), null);
        }).Start();
    }

    /// <summary>
    /// Invoke SlowMethod4 using 3 Parallel.Invoke,
    /// passing the values of the 3 Entry.
    /// Write the results on the 3 Labels.
    /// Use a SynchronizationContext to get back to 
    /// the Thread that owns your labels.
    /// Wait for the sum and update labelSum with the result.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnParallelInvoke_Click(object sender, EventArgs e) {
        SynchronizationContext contextOfMainThread = SynchronizationContext.Current!;
        SlowClass sc = new SlowClass();
        int sum = 0;
        int[] numbers = { int.Parse(txtNumber01.Text), int.Parse(txtNumber02.Text), int.Parse(txtNumber03.Text) };
        Label[] labels = { lblResult01, lblResult02, lblResult03 };

        foreach (Label label in labels) {
            label.Text = string.Empty;
        }
        lblSum.Text = string.Empty;


        ThreadPool.QueueUserWorkItem(_ => {
            Parallel.Invoke(
            () => {
                int result = sc.SlowMethod04(numbers[0]);
                Interlocked.Add(ref sum, result);
                contextOfMainThread.Post(_ => labels[0].Text = result.ToString(), null);
            },
            () => {
                int result = sc.SlowMethod04(numbers[1]);
                Interlocked.Add(ref sum, result);
                contextOfMainThread.Post(_ => labels[1].Text = result.ToString(), null);
            },
            () => {
                int result = sc.SlowMethod04(numbers[2]);
                Interlocked.Add(ref sum, result);
                contextOfMainThread.Post(_ => labels[2].Text = result.ToString(), null);
            });
            contextOfMainThread.Post(_ => lblSum.Text = sum.ToString(), null);
        }, null);
    }

    /// <summary>
    /// Invoke SlowMethod4 using 3 Parallel.For,
    /// passing the values of the 3 Entry.
    /// Write the results on the 3 Labels.
    /// Use a SynchronizationContext to get back to 
    /// the Thread that owns your labels.
    /// Wait for the sum and update labelSum with the result.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnParallelFor_Click(object sender, EventArgs e) {
        SynchronizationContext contextOfMainThread = SynchronizationContext.Current!;
        SlowClass sc = new SlowClass();
        int sum = 0;
        int[] numbers = { int.Parse(txtNumber01.Text), int.Parse(txtNumber02.Text), int.Parse(txtNumber03.Text) };
        Label[] labels = { lblResult01, lblResult02, lblResult03 };

        foreach (Label label in labels) {
            label.Text = string.Empty;
        }
        lblSum.Text = string.Empty;


        ThreadPool.QueueUserWorkItem(_ => {
            Parallel.For(0, numbers.Length, index => {
                int result = sc.SlowMethod04(numbers[index]);
                Interlocked.Add(ref sum, result);
                contextOfMainThread.Post(_ => labels[index].Text = result.ToString(), null);
            });
            contextOfMainThread.Post(_ => lblSum.Text = sum.ToString(), null);
        });
    }

    /// <summary>
    /// Invoke SlowMethod4 using 3 Parallel.ForEach,
    /// passing the values of the 3 Entry.
    /// Write the results on the 3 Labels.
    /// Use a SynchronizationContext to get back to 
    /// the Thread that owns your labels.
    /// Wait for the sum and update labelSum with the result.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnParallelForEach_Click(object sender, EventArgs e) {
        SynchronizationContext contextOfMainThread = SynchronizationContext.Current!;
        SlowClass sc = new SlowClass();
        int sum = 0;
        (int Number, Label Label)[] items = {
                (int.Parse(txtNumber01.Text), lblResult01),
                (int.Parse(txtNumber02.Text), lblResult02),
                (int.Parse(txtNumber03.Text), lblResult03)
            };
        lblResult01.Text = string.Empty;
        lblResult02.Text = string.Empty;
        lblResult03.Text = string.Empty;
        lblSum.Text = string.Empty;
        ThreadPool.QueueUserWorkItem(_ => {
            Parallel.ForEach(items, item => {
                int result = sc.SlowMethod04(item.Number);
                Interlocked.Add(ref sum, result);
                contextOfMainThread.Post(_ => item.Label.Text = result.ToString(), null);
            });
            contextOfMainThread.Post(_ => lblSum.Text = sum.ToString(), null);
        });
    }

    /// <summary>
    /// Invoke SlowMethod4 by creating 3 parallel Tasks and Starting them,
    /// passing the values of the 3 Entry.
    /// Write the results on the 3 Labels.
    /// Use a SynchronizationContext to get back to 
    /// the Thread that owns your labels.
    /// Use a WaitAll to wait for the sum and update labelSum with the result.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnNewTask_Clicked(object sender, EventArgs e) {
        SynchronizationContext contextOfMainThread = SynchronizationContext.Current;
        SlowClass sc = new SlowClass();
        int sum = 0;
        int[] numbers = { int.Parse(txtNumber01.Text), int.Parse(txtNumber02.Text), int.Parse(txtNumber03.Text) };
        Task[] tasks = new Task[3];
        Label[] labels = { lblResult01, lblResult02, lblResult03 };

        foreach (Label label in labels) {
            label.Text = string.Empty;
        }
        lblSum.Text = string.Empty;

        new Task(() => {
            for (int i = 0; i < numbers.Length; i++) {
                int index = i;
                tasks[i] = new Task(() => {
                    int result = sc.SlowMethod04(numbers[index]);
                    Interlocked.Add(ref sum, result);
                    contextOfMainThread.Post(_ => labels[index].Text = result.ToString(), null);
                });
                tasks[i].Start();
            }
            Task.WaitAll(tasks);
            contextOfMainThread.Post(_ => lblSum.Text = sum.ToString(), null);
        }).Start();
    }

    /// <summary>
    /// Invoke SlowMethod4 by Running 3 parallel Tasks,
    /// passing the values of the 3 Entry.
    /// Write the results on the 3 Labels.
    /// Use a SynchronizationContext to get back to 
    /// the Thread that owns your labels.
    /// Use a WaitAll to wait for the sum and update labelSum with the result.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnTaskRun_Clicked(object sender, EventArgs e) {
        SynchronizationContext contextOfMainThread = SynchronizationContext.Current;
        SlowClass sc = new SlowClass();
        int sum = 0;
        int[] numbers = { int.Parse(txtNumber01.Text), int.Parse(txtNumber02.Text), int.Parse(txtNumber03.Text) };
        Task[] tasks = new Task[3];
        Label[] labels = { lblResult01, lblResult02, lblResult03 };

        foreach (Label label in labels) {
            label.Text = string.Empty;
        }
        lblSum.Text = string.Empty;


        Task.Run(() => {
            for (int i = 0; i < numbers.Length; i++) {
                int index = i;
                tasks[i] = Task.Run(() => {
                    int result = sc.SlowMethod04(numbers[index]);
                    Interlocked.Add(ref sum, result);
                    contextOfMainThread.Post(_ => labels[index].Text = result.ToString(), null);
                });
            }
            Task.WaitAll(tasks);
            contextOfMainThread.Post(_ => lblSum.Text = sum.ToString(), null);
        });
    }

    /// <summary>
    /// Invoke SlowMethod4 by running 3 parallel Tasks,
    /// passing the values of the 3 Entry.
    /// Write the results on the 3 Labels using a ContinueWith.
    /// Use a SynchronizationContext to get back to 
    /// the Thread that owns your labels.
    /// Use a WhenAll to wait for the sum and update labelSum with the result in a ContinueWith.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnTasksContinueWith_Click(object sender, EventArgs e) {
        SynchronizationContext contextOfMainThread = SynchronizationContext.Current;
        SlowClass sc = new SlowClass();

        int[] numbers = { int.Parse(txtNumber01.Text), int.Parse(txtNumber02.Text), int.Parse(txtNumber03.Text) };
        Task<int>[] tasks = new Task<int>[3];
        Label[] labels = { lblResult01, lblResult02, lblResult03 };

        foreach (Label label in labels) {
            label.Text = string.Empty;
        }

        lblSum.Text = string.Empty;

        for (int i = 0; i < numbers.Length; i++) {
            int index = i;
            tasks[i] = Task.Run(() => {
                return sc.SlowMethod04(numbers[index]);
            });
            tasks[i].ContinueWith(t => {
                contextOfMainThread.Post(_ => labels[index].Text = t.Result.ToString(), null);
            });
        }
        Task.WhenAll(tasks).ContinueWith(t => {
            contextOfMainThread.Post(_ => lblSum.Text = t.Result.Sum().ToString(), null);
        });
    }

    /// <summary>
    /// Invoke SlowMethod4 by running 3 parallel Tasks,
    /// passing the values of the 3 Entry.
    /// Write the results on the 3 Labels using a ContinueWith 
    /// with the option to run from the CurrentSynchronizationContext.
    /// Use a WhenAll to wait for the sum and update labelSum with the result in a ContinueWith
    /// also using the option to continue from the current synchronization context.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnTasksFromSynchronizationContext_Click(object sender, EventArgs e) {
        SynchronizationContext contextOfMainThread = SynchronizationContext.Current;
        SlowClass sc = new SlowClass();

        int[] numbers = { int.Parse(txtNumber01.Text), int.Parse(txtNumber02.Text), int.Parse(txtNumber03.Text) };
        Task<int>[] tasks = new Task<int>[3];
        Label[] labels = { lblResult01, lblResult02, lblResult03 };

        foreach (Label label in labels) {
            label.Text = string.Empty;
        }

        lblSum.Text = string.Empty;

        for (int i = 0; i < numbers.Length; i++) {
            int index = i;
            tasks[i] = Task.Run(() => sc.SlowMethod04(numbers[index]));
            tasks[i].ContinueWith(t => labels[index].Text = t.Result.ToString() ,TaskScheduler.FromCurrentSynchronizationContext());
        }
        Task.WhenAll(tasks).ContinueWith(t => lblSum.Text = t.Result.Sum().ToString(), TaskScheduler.FromCurrentSynchronizationContext());
    }

    /// <summary>
    /// Invoke SlowMethod4 by running 3 parallel Tasks,
    /// passing the values of the 3 Entry.
    /// Write the results on the 3 Labels by invoking a separate method that awaits on the Task.
    /// Use a WhenAll to await for the sum and update labelSum with the result 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void btnTasksAwait_Click(object sender, EventArgs e) {
        SlowClass sc = new SlowClass();

        int[] numbers = { int.Parse(txtNumber01.Text), int.Parse(txtNumber02.Text), int.Parse(txtNumber03.Text) };
        Task<int>[] tasks = new Task<int>[3];
        Label[] labels = { lblResult01, lblResult02, lblResult03 };

        foreach (Label label in labels) {
            label.Text = string.Empty;
        }

        lblSum.Text = string.Empty;

        for (int i = 0; i < numbers.Length; i++) {
            int index = i;
            tasks[i] = Task.Run(() => {
                return sc.SlowMethod04(numbers[index]);
            });

            AwaiterMethod(tasks[i], labels[i]);

        }
        int[] results = await Task.WhenAll(tasks);
        lblSum.Text = results.Sum().ToString();
    }

    async void AwaiterMethod(Task<int> t, Label label) {
        label.Text = (await t).ToString();
    }

    /// <summary>
    /// Create a new SlowLibraryCacheService.GetSlowSquare(int number).
    /// This method should Run the SlowMethod4 in a Task and save the result
    /// in a cache. Use a ThreadSafe collection to store the value.
    /// If the GetSlowSquare method is invoked with an input that has been previously retrieved,
    /// return that result immediately instead of invoking the SlowMethod again.
    /// Invoke GetSlowSquare 3 times 
    /// passing the values of the 3 Entry.
    /// Write the results on the 3 Labels by invoking a separate method that awaits on the Task.
    /// Use a WhenAll to await for the sum and update labelSum with the result 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void btnTaskFromResult_Click(object sender, EventArgs e) {
        int[] numbers = { int.Parse(txtNumber01.Text), int.Parse(txtNumber02.Text), int.Parse(txtNumber03.Text) };
        Task<int>[] tasks = new Task<int>[3];
        Label[] labels = { lblResult01, lblResult02, lblResult03 };

        foreach (Label label in labels) {
            label.Text = string.Empty;
        }

        lblSum.Text = string.Empty;

        for (int i = 0; i < numbers.Length; i++) {
            tasks[i] = SlowLibraryCacheService.GetSlowSquare(numbers[i]);
            AwaiterMethod(tasks[i], labels[i]);
        }
        int[] results = await Task.WhenAll(tasks);
        lblSum.Text = results.Sum().ToString();
    }

    /// <summary>
    /// Invoke SlowMethod4 by running 3 parallel Tasks,
    /// passing the values of the 3 Entry.
    /// Store the 3 tasks in a collection, await on the fastest, 
    /// update the corresponding label with the result and remove the completed task from the collection.
    /// Repeat until the collection is empty.
    /// Use a WhenAll to await for the sum and update labelSum with the result 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void btnAwaitWhenAny_Click(object sender, EventArgs e) {
        SlowClass slowClass = new SlowClass();
        int[] numbers = { int.Parse(txtNumber01.Text), int.Parse(txtNumber02.Text), int.Parse(txtNumber03.Text) };
        Label[] labels = { lblResult01, lblResult02, lblResult03 };

        foreach (Label label in labels) {
            label.Text = string.Empty;
        }

        lblSum.Text = string.Empty;

        List<Task<int>> tasks = new();
        Dictionary<Task<int>, Label> taskDictionary = new();
        for (int i = 0; i < numbers.Length; i++) {
            int index = i;
            tasks.Add(Task.Run(() => slowClass.SlowMethod04(numbers[index])));
            taskDictionary.Add(tasks[index], labels[index]);
        }
        while (taskDictionary.Any()) {
            Task<int> t = await Task.WhenAny(taskDictionary.Select(td=>td.Key));
            taskDictionary[t].Text = (await t).ToString();
            taskDictionary.Remove(t);
        }

        int[] result = await Task.WhenAll(tasks);
        lblSum.Text = result.Sum().ToString();
    }

    /// <summary>
    /// Invoke SlowMethod4 by running 3 parallel Tasks,
    /// passing the values of the 3 Entry.
    /// Store the 3 tasks in a collection, pass the collection to the Interleaved method in the TasksUtilities class.
    /// await on each of the tasks resulting from the Interleaved method
    /// and update the corresponding label with the result.
    /// Use a WhenAll to await for the sum and update labelSum with the result 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void btnInterleaved_Click(object sender, EventArgs e) {
        SlowClass slowClass = new SlowClass();
        int[] numbers = { int.Parse(txtNumber01.Text), int.Parse(txtNumber02.Text), int.Parse(txtNumber03.Text) };
        Label[] labels = { lblResult01, lblResult02, lblResult03 };

        foreach (Label label in labels) {
            label.Text = string.Empty;
        }

        lblSum.Text = string.Empty;

        Task<(int number, Label label)>[] tasks = new Task<(int number, Label label)>[3];
        for (int i = 0; i < numbers.Length; i++) {
            int index = i;
            tasks[i] = Task.Run(() => (number: slowClass.SlowMethod04(numbers[index]), label: labels[index]));
        }

        var tasksInCorrectOrder = new TasksUtilities().Interleaved(tasks);
        foreach (var item in tasksInCorrectOrder) {
            var r = await await item;
            r.label.Text = r.number.ToString();
        }

        (int number, Label label)[] result = await Task.WhenAll(tasks);
        lblSum.Text = result.Select(t => t.number).Sum().ToString();
    }

    /// <summary>
    /// Invoke SlowMethod4 by running 3 parallel Tasks,
    /// passing the values of the 3 Entry.
    /// Store the 3 tasks in a collection, pass the collection to the InterleavedStream method in the TasksUtilities class.
    /// await on each of the tasks resulting from the InterleavedStream method, 
    /// and update the corresponding label with the result.
    /// Use a WhenAll to await for the sum and update labelSum with the result 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void btnInterleavedStream_Clicked(object sender, EventArgs e) {
        SlowClass slowClass = new SlowClass();
        int[] numbers = { int.Parse(txtNumber01.Text), int.Parse(txtNumber02.Text), int.Parse(txtNumber03.Text) };
        Label[] labels = { lblResult01, lblResult02, lblResult03 };

        foreach (Label label in labels) {
            label.Text = string.Empty;
        }

        lblSum.Text = string.Empty;

        Task<(int number, Label label)>[] tasks = new Task<(int number, Label label)>[3];
        for (int i = 0; i < numbers.Length; i++) {
            int index = i;
            tasks[i] = Task.Run(() => (number: slowClass.SlowMethod04(numbers[index]), label: labels[index]));
        }

        await foreach (Task<(int number, Label label)> item in new TasksUtilities().InterleavedStream(tasks)) {
            (int number, Label label) r = await item;
            r.label.Text = r.number.ToString();
        }

        (int number, Label label)[] result = await Task.WhenAll(tasks);
        lblSum.Text = result.Select(t => t.number).Sum().ToString();
    }
}

