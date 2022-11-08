namespace SlowLibrary;
public class TasksUtilities {
    /// <summary>
    /// Create an Interleaved method that gets a collection of Tasks.
    /// Create TaskCompletionSource instances to represent buckets, 
    /// one bucket per each of the tasks that will eventually complete.
    /// Then hook up a continuation to each input task: 
    /// this continuation will get the next available bucket and store the newly completed task into it.
    /// </summary>
    ///<param name="tasksToWaitOn">Original collection of Tasks to wait on</param>
    ///<returns>A collection of Tasks that will complete in ascending order</returns>
    public IEnumerable<Task<Task<T>>> Interleaved<T>(IEnumerable<Task<T>> tasksToWaitOn) {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Create an InterleavedStream method that gets a collection of Tasks.
    /// This method returns an IAsyncEnumerable<Task<T>>
    /// Invoke the Interleaved method passing your input as an argument.
    /// foreach item in the result of the Interleaved method, 
    /// await the item and yield return the result
    /// </summary>
    ///<param name="tasksToWaitOn">Original collection of Tasks to wait on</param>
    ///<returns>An IAsyncEnumerable of Tasks that will complete in ascending order</returns>
    public async IAsyncEnumerable<Task<T>> InterleavedStream<T>(IEnumerable<Task<T>> tasksToWaitOn) {
        throw new NotImplementedException();
    }
}
