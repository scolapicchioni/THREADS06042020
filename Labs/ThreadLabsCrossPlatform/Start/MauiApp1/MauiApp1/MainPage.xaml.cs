using SlowLibrary;
namespace MauiApp1;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    private void CleanLabels() {
        lblResult01.Text = string.Empty;
        lblResult02.Text = string.Empty;
        lblResult03.Text = string.Empty;
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
        CleanLabels();
        //TODO: add your code here

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
        CleanLabels();
        //TODO: add your code here

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
        CleanLabels();
        //TODO: add your code here

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
        CleanLabels();
        //TODO: add your code here

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
        CleanLabels();
        //TODO: add your code here

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
        CleanLabels();
        //TODO: add your code here

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
        CleanLabels();
        //TODO: add your code here

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
        CleanLabels();
        //TODO: add your code here

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
        CleanLabels();
        //TODO: add your code here

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
        CleanLabels();
        //TODO: add your code here

    }

    /// <summary>
    /// Invoke SlowMethod4 by running 3 parallel Tasks,
    /// passing the values of the 3 Entry.
    /// Write the results on the 3 Labels by invoking a separate method that awaits on the Task.
    /// Use a WhenAll to await for the sum and update labelSum with the result 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnTasksAwait_Click(object sender, EventArgs e) {
        CleanLabels();
        //TODO: add your code here

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
    private void btnTaskFromResult_Click(object sender, EventArgs e) {
        CleanLabels();
        //TODO: add your code here

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
    private void btnAwaitWhenAny_Click(object sender, EventArgs e) {
        CleanLabels();
        //TODO: add your code here

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
    private void btnInterleaved_Click(object sender, EventArgs e) {
        CleanLabels();
        //TODO: add your code here

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
    private void btnInterleavedStream_Clicked(object sender, EventArgs e) {
        CleanLabels();
        //TODO: add your code here

    }
}

