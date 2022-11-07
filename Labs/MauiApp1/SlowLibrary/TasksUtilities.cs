using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlowLibrary {
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
            List<TaskCompletionSource<Task<T>>> taskCompletionSources = new();
            int index = 0;
            foreach (var task in tasksToWaitOn) {
                taskCompletionSources.Add(new());
                task.ContinueWith(t => {
                    taskCompletionSources[index].SetResult(t);
                    Interlocked.Increment(ref index);
                });
            }
            return taskCompletionSources.Select(tcs => tcs.Task);
        }
    }
}
